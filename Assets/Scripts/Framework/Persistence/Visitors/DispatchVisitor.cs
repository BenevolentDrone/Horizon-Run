using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class DispatchVisitor
		: ISaveVisitor,
		  ILoadVisitor,
		  IPopulateVisitor
	{
		private readonly IReadOnlyRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository;

		private readonly ILogger logger;

		public DispatchVisitor(
			IReadOnlyRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository,
			ILogger logger = null)
		{
			this.concreteVisitorRepository = concreteVisitorRepository;

			this.logger = logger;
		}

		#region IVisitor

		#region Can visit

		public bool CanVisit<TVisitable>()
		{
			return concreteVisitorRepository.Has(
				typeof(TVisitable));
		}

		public bool CanVisit(
			Type visitableType)
		{
			return concreteVisitorRepository.Has(
				visitableType);
		}

		#endregion

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				typeof(TVisitable),
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				dto = null;

				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
				{
					if (concreteSaveVisitor.VisitSave<TVisitable>(
						ref dto,
						visitable))
					{
						return true;
					}
					
					logger?.LogError(
						GetType(),
						$"FAILED TO SAVE VISITABLE TYPE: {typeof(TVisitable).Name}");

					dto = null;

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE SAVE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			dto = default;

			return false;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			IVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				visitableType,
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

				dto = null;

				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
				{
					if (concreteSaveVisitor.VisitSave(
						ref dto,
						visitableType,
						visitable))
					{
						return true;
					}

					logger?.LogError(
						GetType(),
						$"FAILED TO SAVE VISITABLE TYPE: {visitableType.Name}");

					dto = null;

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE SAVE VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			dto = default;

			return false;
		}

		#endregion

		#region ILoadVisitor

		public bool VisitLoad<TVisitable>(
			object dto,
			out TVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				typeof(TVisitable),
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				visitable = default;

				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is ILoadVisitor concreteLoadVisitor)
				{
					if (concreteLoadVisitor.VisitLoad<TVisitable>(
						dto,
						out var nonCastedVisitable))
					{
						if (nonCastedVisitable is TVisitable castedVisitable)
						{
							visitable = castedVisitable;

							return true;
						}
						
						logger?.LogError(
							GetType(),
							$"FAILED TO CAST VISITABLE {nonCastedVisitable.GetType().Name} TO TYPE: {typeof(TVisitable).Name}");

						visitable = default;

						return false;
					}
					
					logger?.LogError(
						GetType(),
						$"FAILED TO LOAD VISITABLE TYPE: {typeof(TVisitable).Name}");

					visitable = default;

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE LOAD VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			visitable = default;

			return false;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out IVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				visitableType,
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

				visitable = default;

				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is ILoadVisitor concreteLoadVisitor)
				{
					if (concreteLoadVisitor.VisitLoad(
						dto,
						visitableType,
						out visitable))
					{
						return true;
					}

					logger?.LogError(
						GetType(),
						$"FAILED TO LOAD VISITABLE TYPE: {visitableType.Name}");

					visitable = default;

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE LOAD VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			visitable = default;

			return false;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				typeof(TVisitable),
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				return false;
			}

			IVisitable visitableDowncasted = visitable as IVisitable;

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is IPopulateVisitor concretePopulateVisitor)
				{
					if (concretePopulateVisitor.VisitPopulate(
						dto,
						visitableDowncasted))
					{
						return true;
					}
					
					logger?.LogError(
						GetType(),
						$"FAILED TO POPULATE VISITABLE TYPE: {typeof(TVisitable).Name}");

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE POPULATE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			return false;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			IVisitable visitable)
		{
			if (!concreteVisitorRepository.TryGet(
				visitableType,
				out IEnumerable<IVisitor> concreteVisitors))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is IPopulateVisitor concretePopulateVisitor)
				{
					if (concretePopulateVisitor.VisitPopulate(
						dto,
						visitable))
					{
						return true;
					}

					logger?.LogError(
						GetType(),
						$"FAILED TO POPULATE VISITABLE TYPE: {visitableType.Name}");

					return false;
				}
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE POPULATE VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			return false;
		}

		#endregion
	}
}