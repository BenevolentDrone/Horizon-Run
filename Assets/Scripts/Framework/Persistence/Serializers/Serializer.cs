using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class Serializer
		: ISerializer
	{
		private readonly IReadOnlySerializerContext context;

		private readonly ILogger logger;

		public Serializer(
			IReadOnlySerializerContext context,
			ILogger logger = null)
		{
			this.context = context;

			this.logger = logger;
		}

		#region ISerializer

		public IReadOnlySerializerContext Context { get => context; }

		#region Serialize

		public bool Serialize<TValue>(
			TValue value)
		{
			if (!context.Visitor.CanVisit<TValue>())
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {typeof(TValue).Name}");

				return false;
			}

			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			try
			{
				object dto = null;

				if (!saveVisitor.VisitSave<TValue>(
					ref dto,
					value))
				{
					logger?.LogError(
						GetType(),
						$"VISIT SAVE FAILED: {typeof(TValue).Name}");
	
					return false;
				}

				if (!context.FormatSerializer.Serialize(
					context.SerializationStrategy,
					context.Arguments,
					dto.GetType(),
					dto))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {dto.GetType().Name}");

					return false;
				}
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}

			return true;
		}

		public bool Serialize(
			Type valueType,
			object valueObject)
		{
			if (!context.Visitor.CanVisit(valueType))
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {valueType.Name}");

				return false;
			}

			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			try
			{
				object dto = null;

				if (!saveVisitor.VisitSave(
					ref dto,
					valueType,
					valueObject as IVisitable))
				{
					logger?.LogError(
						GetType(),
						$"VISIT SAVE FAILED: {valueType.Name}");

					return false;
				}

				if (!context.FormatSerializer.Serialize(
					context.SerializationStrategy,
					context.Arguments,
					dto.GetType(),
					dto))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {dto.GetType().Name}");

					return false;
				}
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}

			return true;
		}

		#endregion

		#region Deserialize

		public bool Deserialize<TValue>(
			out TValue value)
		{
			value = default;

			if (!context.Visitor.CanVisit<TValue>())
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {typeof(TValue).Name}");

				return false;
			}

			var loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			Type dtoType = loadVisitor.GetDTOType<TValue>();

			try
			{
				object dto = null;

				if (!context.FormatSerializer.Deserialize(
					context.SerializationStrategy,
					context.Arguments,
					dtoType,
					out dto))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {dtoType.Name}");

					return false;
				}

				if (!loadVisitor.VisitLoad<TValue>(
					dto,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"VISIT LOAD FAILED: {typeof(TValue).Name}");

					return false;
				}

			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return false;
			}

			return true;
		}

		public bool Deserialize(
			Type valueType,
			out object valueObject)
		{
			valueObject = default;

			if (!context.Visitor.CanVisit(valueType))
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {valueType.Name}");

				return false;
			}

			var loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			Type dtoType = loadVisitor.GetDTOType(valueType);

			try
			{
				object dto = null;

				if (!context.FormatSerializer.Deserialize(
					context.SerializationStrategy,
					context.Arguments,
					dtoType,
					out dto))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {dtoType.Name}");

					return false;
				}

				if (!loadVisitor.VisitLoad(
					dto,
					valueType,
					out IVisitable valueVisitable))
				{
					logger?.LogError(
						GetType(),
						$"VISIT LOAD FAILED: {valueType.Name}");

					return false;
				}

				valueObject = valueVisitable;

			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return false;
			}

			return true;
		}

		#endregion

		#region Populate

		public bool Populate<TValue>(
			ref TValue value)
		{
			if (!context.Visitor.CanVisit<TValue>())
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {typeof(TValue).Name}");

				return false;
			}

			var populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			Type dtoType = populateVisitor.GetDTOType<TValue>();

			try
			{
				object dto = null;

				if (!context.FormatSerializer.Deserialize(
					context.SerializationStrategy,
					context.Arguments,
					dtoType,
					out dto))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {dtoType.Name}");

					return false;
				}

				if (!populateVisitor.VisitPopulate<TValue>(
					dto,
					value))
				{
					logger?.LogError(
						GetType(),
						$"VISIT POPULATE FAILED: {typeof(TValue).Name}");

					return false;
				}

			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}

			return true;
		}

		public bool Populate(
			Type valueType,
			ref object valueObject)
		{
			if (!context.Visitor.CanVisit(valueType))
			{
				logger?.LogError(
					GetType(),
					$"CANNOT VISIT TYPE: {valueType.Name}");

				return false;
			}

			var populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			Type dtoType = populateVisitor.GetDTOType(valueType);

			try
			{
				object dto = null;

				if (!context.FormatSerializer.Deserialize(
					context.SerializationStrategy,
					context.Arguments,
					dtoType,
					out dto))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {dtoType.Name}");

					return false;
				}

				if (!populateVisitor.VisitPopulate(
					dto,
					valueType,
					valueObject as IVisitable))
				{
					logger?.LogError(
						GetType(),
						$"VISIT POPULATE FAILED: {valueType.Name}");

					return false;
				}

			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}

			return true;
		}

		#endregion

		#endregion
	}
}