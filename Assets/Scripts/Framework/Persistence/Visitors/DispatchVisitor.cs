using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class CompositeVisitor
		: ISaveVisitorGeneric,
		  ILoadVisitorGeneric,
		  IPopulateVisitorGeneric
	{
		private readonly IReadOnlyRepository<Type, IConcreteVisitor[]> concreteVisitorRepository;

		private readonly ILogger logger;

		public CompositeVisitor(
			IReadOnlyRepository<Type, IConcreteVisitor[]> concreteVisitorRepository,
			ILogger logger = null)
		{
			this.concreteVisitorRepository = concreteVisitorRepository;

			this.logger = logger;
		}

		#region ISaveVisitorGeneric

		public bool Save<TConcreteVisitable, TDTO>(
			TConcreteVisitable visitable,
			out TDTO DTO)
		{
			if (concreteVisitorRepository.TryGet(
				typeof(TConcreteVisitable),
				out IConcreteVisitor[] concreteVisitors))
			{
				foreach (IConcreteVisitor concreteVisitor in concreteVisitors)
				{
					if (concreteVisitor is ISaveVisitor<TDTO> concreteSaveVisitor)
					{
						if (concreteSaveVisitor.Save(
							visitable as IVisitable,
							out DTO))
						{
							return true;
						}
						else
						{
							logger?.LogError(
								GetType(),
								$"FAILED TO SAVE VISITABLE TYPE: {typeof(TConcreteVisitable).Name} USING DTO TYPE: {typeof(TDTO).Name}");

							DTO = default;

							return false;
						}
					}
				}

				logger?.LogError(
					GetType(),
					$"NO CONCRETE SAVE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name} AND DTO TYPE: {typeof(TDTO).Name}");

				DTO = default;

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name}");

			DTO = default;

			return false;
		}

		#endregion

		#region ILoadVisitorGeneric

		public bool Load<TConcreteVisitable, TDTO>(
			TDTO DTO,
			out TConcreteVisitable visitable)
		{
			if (concreteVisitorRepository.TryGet(
				typeof(TConcreteVisitable),
				out IConcreteVisitor[] concreteVisitors))
			{
				foreach (IConcreteVisitor concreteVisitor in concreteVisitors)
				{
					if (concreteVisitor is ILoadVisitor<TDTO> concreteLoadVisitor)
					{
						if (concreteLoadVisitor.Load(
							DTO,
							out var nonCastedVisitable))
						{
							if (nonCastedVisitable is TConcreteVisitable castedVisitable)
							{
								visitable = castedVisitable;

								return true;
							}
							else
							{
								logger?.LogError(
									GetType(),
									$"FAILED TO CAST VISITABLE {nonCastedVisitable.GetType().Name} TO TYPE: {typeof(TConcreteVisitable).Name} USING DTO TYPE: {typeof(TDTO).Name}");

								visitable = default;

								return false;
							}
						}
						else
						{
							logger?.LogError(
								GetType(),
								$"FAILED TO LOAD VISITABLE TYPE: {typeof(TConcreteVisitable).Name} USING DTO TYPE: {typeof(TDTO).Name}");

							visitable = default;

							return false;
						}
					}
				}

				logger?.LogError(
					GetType(),
					$"NO CONCRETE LOAD VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name} AND DTO TYPE: {typeof(TDTO).Name}");

				visitable = default;

				return false;
			}
			
			logger?.LogError(
				GetType(),
				$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name}");

			visitable = default;

			return false;
		}

		#endregion

		#region IPopulateVisitorGeneric

		public bool Populate<TConcreteVisitable, TDTO>(
			TDTO DTO,
			ref TConcreteVisitable visitable)
		{
			if (concreteVisitorRepository.TryGet(
				typeof(TConcreteVisitable),
				out IConcreteVisitor[] concreteVisitors))
			{
				IVisitable visitableDowncasted = visitable as IVisitable;

				foreach (IConcreteVisitor concreteVisitor in concreteVisitors)
				{
					if (concreteVisitor is IPopulateVisitor<TDTO> concretePopulateVisitor)
					{
						if (concretePopulateVisitor.Populate(
							DTO,
							ref visitableDowncasted))
						{
							return true;
						}
						else
						{
							logger?.LogError(
								GetType(),
								$"FAILED TO POPULATE VISITABLE TYPE: {typeof(TConcreteVisitable).Name} USING DTO TYPE: {typeof(TDTO).Name}");

							return false;
						}
					}
				}

				logger?.LogError(
					GetType(),
					$"NO CONCRETE POPULATE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name} AND DTO TYPE: {typeof(TDTO).Name}");

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TConcreteVisitable).Name}");

			return false;
		}

		#endregion
	}
}