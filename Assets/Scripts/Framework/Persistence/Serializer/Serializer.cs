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
			ILogger logger)
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
			try
			{
				if (context.Visitor.CanVisit<TValue>())
				{
					object dto = null;

					var saveVisitor = context.Visitor as ISaveVisitor;

					if (saveVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

						return false;
					}

					if (!saveVisitor.VisitSave<TValue>(
						ref dto,
						value))
					{
						logger?.LogError(
							GetType(),
							$"VISIT SAVE FAILED: {nameof(TValue)}");

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
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {nameof(TValue)}, TRYING TO SERIALIZE AS DTO");

					if (!context.FormatSerializer.Serialize<TValue>(
						context.SerializationStrategy,
						context.Arguments,
						value))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {nameof(TValue)}");

						return false;
					}
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
			try
			{
				if (context.Visitor.CanVisit(valueType))
				{
					object dto = null;

					var saveVisitor = context.Visitor as ISaveVisitor;

					if (saveVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

						return false;
					}

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
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {valueType.Name}, TRYING TO SERIALIZE AS DTO");

					if (!context.FormatSerializer.Serialize(
						context.SerializationStrategy,
						context.Arguments,
						valueType,
						valueObject))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {valueType.Name}");

						return false;
					}
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

			try
			{
				if (context.Visitor.CanVisit<TValue>())
				{
					var loadVisitor = context.Visitor as ILoadVisitor;
	
					if (loadVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");
	
						return false;
					}
	
					Type dtoType = loadVisitor.GetDTOType<TValue>();
	
					if (!context.FormatSerializer.Deserialize(
						context.SerializationStrategy,
						context.Arguments,
						dtoType,
						out object dto))
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
							$"VISIT LOAD FAILED: {nameof(TValue)}");
	
						return false;
					}
				}
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {nameof(TValue)}, TRYING TO DESERIALIZE AS DTO");

					var loadVisitor = context.Visitor as ILoadVisitor;

					if (loadVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

						return false;
					}

					if (!context.FormatSerializer.Deserialize<TValue>(
						context.SerializationStrategy,
						context.Arguments,
						out value))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {nameof(TValue)}");

						return false;
					}
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

			try
			{
				if (context.Visitor.CanVisit(valueType))
				{
					var loadVisitor = context.Visitor as ILoadVisitor;
	
					if (loadVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");
	
						return false;
					}
	
					Type dtoType = loadVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						context.SerializationStrategy,
						context.Arguments,
						dtoType,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");
	
						return false;
					}
	
					if (!loadVisitor.VisitLoad(
						dto,
						valueType,
						out valueObject))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");
	
						return false;
					}
				}
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {valueType.Name}, TRYING TO DESERIALIZE AS DTO");

					var loadVisitor = context.Visitor as ILoadVisitor;

					if (loadVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

						return false;
					}

					if (!context.FormatSerializer.Deserialize(
						context.SerializationStrategy,
						context.Arguments,
						valueType,
						out valueObject))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {valueType.Name}");

						return false;
					}
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

		#endregion

		#region Populate

		public bool Populate<TValue>(
			ref TValue value)
		{
			try
			{
				if (context.Visitor.CanVisit<TValue>())
				{
					var populateVisitor = context.Visitor as IPopulateVisitor;
	
					if (populateVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");
	
						return false;
					}
	
					Type dtoType = populateVisitor.GetDTOType<TValue>();
	
					if (!context.FormatSerializer.Deserialize(
						context.SerializationStrategy,
						context.Arguments,
						dtoType,
						out object dto))
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
							$"VISIT POPULATE FAILED: {nameof(TValue)}");
	
						return false;
					}
				}
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {nameof(TValue)}, TRYING TO POPULATE AS DTO");
	
					var populateVisitor = context.Visitor as IPopulateVisitor;
	
					if (populateVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");
	
						return false;
					}
	
					if (!context.FormatSerializer.Populate<TValue>(
						context.SerializationStrategy,
						context.Arguments,
						ref value))
					{
						logger?.LogError(
							GetType(),
							$"POPULATING FAILED: {nameof(TValue)}");
	
						return false;
					}
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
			try
			{
				if (context.Visitor.CanVisit(valueType))
				{
					var populateVisitor = context.Visitor as IPopulateVisitor;
	
					if (populateVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");
	
						return false;
					}
	
					Type dtoType = populateVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						context.SerializationStrategy,
						context.Arguments,
						dtoType,
						out object dto))
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
				else
				{
					logger?.Log(
						GetType(),
						$"CANNOT VISIT TYPE: {valueType.Name}, TRYING TO POPULATE AS DTO");

					var populateVisitor = context.Visitor as IPopulateVisitor;

					if (populateVisitor == null)
					{
						logger?.LogError(
							GetType(),
							$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

						return false;
					}

					if (!context.FormatSerializer.Populate(
						context.SerializationStrategy,
						context.Arguments,
						valueType,
						ref valueObject))
					{
						logger?.LogError(
							GetType(),
							$"POPULATING FAILED: {valueType.Name}");

						return false;
					}
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