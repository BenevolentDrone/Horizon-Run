using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class Serializer
		: ISerializer,
		  IBlockSerializer,
		  IAsyncSerializer,
		  IAsyncBlockSerializer
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

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			if (context.SerializationStrategy is
				IHasIODestination strategyWithIODestination)
			{
				strategyWithIODestination.EnsureIODestinationExists();
			}
		}

		public bool IODestinationExists()
		{
			if (context.SerializationStrategy is
				IHasIODestination strategyWithIODestination)
			{
				return strategyWithIODestination.IODestinationExists();
			}

			return false;
		}

		public void CreateIODestination()
		{
			if (context.SerializationStrategy is
				IHasIODestination strategyWithIODestination)
			{
				strategyWithIODestination.CreateIODestination();
			}
		}

		public void EraseIODestination()
		{
			if (context.SerializationStrategy is
				IHasIODestination strategyWithIODestination)
			{
				strategyWithIODestination.EraseIODestination();
			}
		}

		#endregion

		#region IHasReadWriteControl

		public bool SupportsSimultaneousReadAndWrite
		{
			get
			{
				if (context.SerializationStrategy is
					IHasReadWriteControl strategyWithReadWriteControl)
				{
					return strategyWithReadWriteControl.SupportsSimultaneousReadAndWrite;
				}

				return false;
			}
		}

		public void InitializeRead()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.InitializeRead();
			}
		}

		public void FinalizeRead()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.FinalizeRead();
			}
		}


		public void InitializeWrite()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.InitializeWrite();
			}
		}

		public void FinalizeWrite()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.FinalizeWrite();
			}
		}


		public void InitializeAppend()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.InitializeAppend();
			}
		}

		public void FinalizeAppend()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.FinalizeAppend();
			}
		}


		public void InitializeReadAndWrite()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.InitializeReadAndWrite();
			}
		}

		public void FinalizeReadAndWrite()
		{
			if (context.SerializationStrategy is
				IHasReadWriteControl strategyWithReadWriteControl)
			{
				strategyWithReadWriteControl.FinalizeReadAndWrite();
			}
		}

		#endregion

		public IReadOnlySerializerContext Context { get => context; }

		#region Serialize

		public bool Serialize<TValue>(
			TValue value)
		{
			try
			{
				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!context.FormatSerializer.Serialize(
						dto.GetType(),
						context,
						dto))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!context.FormatSerializer.Serialize<TValue>(
					context,
					value))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}
		}

		public bool Serialize(
			Type valueType,
			object valueObject)
		{
			try
			{
				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!context.FormatSerializer.Serialize(
						dto.GetType(),
						context,
						dto))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!context.FormatSerializer.Serialize(
					valueType,
					context,
					valueObject))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;			
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public bool Deserialize<TValue>(
			out TValue value)
		{
			value = default;

			try
			{
				if (TryGetLoadVisitor<TValue>(
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>();
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
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

					return true;
				}

				if (!context.FormatSerializer.Deserialize<TValue>(
					context,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return false;
			}
		}

		public bool Deserialize(
			Type valueType,
			out object valueObject)
		{
			valueObject = default;

			try
			{
				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
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

					return true;
				}

				if (!context.FormatSerializer.Deserialize(
					valueType,
					context,
					out valueObject))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Populate

		public bool Populate<TValue>(
			TValue value)
		{
			try
			{
				if (TryGetPopulateVisitor<TValue>(
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>();
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");
	
						return false;
					}

					return TryPopulate<TValue>(
						value,
						dto,
						populateVisitor);
				}
				
				if (!context.FormatSerializer.Populate<TValue>(
					context,
					value))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}
		}

		public bool Populate(
			Type valueType,
			object valueObject)
		{
			try
			{
				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");
	
						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						dto,
						populateVisitor);
				}
				
				if (!context.FormatSerializer.Populate(
					valueType,
					context,
					valueObject))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		#region IBlockSerializer

		#region Serialize

		public bool SerializeBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!blockFormatSerializer.SerializeBlock(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.SerializeBlock<TValue>(
					context,
					value,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool SerializeBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!blockFormatSerializer.SerializeBlock(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.SerializeBlock(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public bool DeserializeBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			value = default;

			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetLoadVisitor<TValue>(
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>();

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

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

					return true;
				}

				if (!blockFormatSerializer.DeserializeBlock<TValue>(
					context,
					blockOffset,
					blockSize,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool DeserializeBlock(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			valueObject = default;

			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

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

					return true;
				}

				if (!blockFormatSerializer.DeserializeBlock(
					valueType,
					context,
					blockOffset,
					blockSize,
					out valueObject))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Populate

		public bool PopulateBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor<TValue>(
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>();

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate<TValue>(
						value,
						dto,
						populateVisitor);
				}

				if (!blockFormatSerializer.PopulateBlock<TValue>(
					context,
					value,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool PopulateBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						dto,
						populateVisitor);
				}

				if (!blockFormatSerializer.PopulateBlock(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		#region IAsyncSerializer

		#region Serialize

		public async Task<bool> SerializeAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					return Serialize<TValue>(
						value);
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!await asyncFormatSerializer.SerializeAsync(
						dto.GetType(),
						context,
						dto,
						
						asyncContext))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncFormatSerializer.SerializeAsync<TValue>(
					context,
					value,
					
					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> SerializeAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					return Serialize(
						valueType,
						valueObject);
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!await asyncFormatSerializer.SerializeAsync(
						dto.GetType(),
						context,
						dto,
						
						asyncContext))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncFormatSerializer.SerializeAsync(
					valueType,
					context,
					valueObject,
					
					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var asyncFormatSerializer = context.FormatSerializer
				as IAsyncFormatSerializer;

			if (asyncFormatSerializer == null)
			{
				bool syncResult = Deserialize<TValue>(
					out value);

				return (syncResult, value);
			}

			try
			{
				if (TryGetLoadVisitor<TValue>(
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>();

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, value);
					}

					if (!loadVisitor.VisitLoad<TValue>(
						asyncResult1.Item2,
						out value))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {nameof(TValue)}");

						return (false, value);
					}

					return (true, value);
				}

				var asyncResult2 = await asyncFormatSerializer.DeserializeAsync<TValue>(
					context,
					
					asyncContext);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {nameof(TValue)}");

					return (false, value);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return (false, value);
			}
		}

		public async Task<(bool, object)> DeserializeAsync(
			Type valueType,
			
			//Async tail
			AsyncExecutionContext asyncContext)                                       
		{
			object valueObject = default;

			var asyncFormatSerializer = context.FormatSerializer
				as IAsyncFormatSerializer;

			if (asyncFormatSerializer == null)
			{
				bool syncResult = Deserialize(
					valueType,
					out valueObject);

				return (syncResult, valueObject);
			}

			try
			{
				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, valueObject);
					}

					if (!loadVisitor.VisitLoad(
						asyncResult1.Item2,
						valueType,
						out valueObject))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						return (false, valueObject);
					}

					return (true, asyncResult1.Item2);
				}

				var asyncResult2 = await asyncFormatSerializer.DeserializeAsync(
					valueType,
					context,
					
					asyncContext);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {valueType.Name}");

					return (false, valueObject);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				return (false, default);
			}
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					return Populate<TValue>(
						value);
				}

				if (TryGetPopulateVisitor<TValue>(
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>();

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate<TValue>(
						value,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncFormatSerializer.PopulateAsync<TValue>(
					context,
					value,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> PopulateAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					return Populate(
						valueType,
						valueObject);
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncFormatSerializer.PopulateAsync(
					valueType,
					context,
					valueObject,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializer

		#region Serialize

		public async Task<bool> SerializeBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						return SerializeBlock<TValue>(
							value,
							blockOffset,
							blockSize);
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize,

						asyncContext))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncBlockFormatSerializer.SerializeBlockAsync<TValue>(
					context,
					value,
					blockOffset,
					blockSize,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> SerializeBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						return SerializeBlock(
							valueType,
							valueObject,
							blockOffset,
							blockSize);
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize,

						asyncContext))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var asyncBlockFormatSerializer = context.FormatSerializer
				as IAsyncBlockFormatSerializer;

			if (asyncBlockFormatSerializer == null)
			{
				if (context.FormatSerializer is IBlockFormatSerializer)
				{
					bool syncResult = DeserializeBlock<TValue>(
						blockOffset,
						blockSize,
						out value);

					return (syncResult, value);
				}
				else
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return (false, value);
				}
			}
			
			try
			{
				if (TryGetLoadVisitor<TValue>(
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>();

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, value);
					}

					if (!loadVisitor.VisitLoad<TValue>(
						asyncResult1.Item2,
						out value))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {nameof(TValue)}");

						return (false, value);
					}

					return (true, value);
				}

				var asyncResult2 = await asyncBlockFormatSerializer.
					DeserializeBlockAsync<TValue>(
						context,
						blockOffset,
						blockSize,
	
						asyncContext);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {nameof(TValue)}");

					return (false, value);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return (false, value);
			}
		}

		public async Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object valueObject = default;

			var asyncBlockFormatSerializer = context.FormatSerializer
				as IAsyncBlockFormatSerializer;

			if (asyncBlockFormatSerializer == null)
			{
				if (context.FormatSerializer is IBlockFormatSerializer)
				{
					bool syncResult = DeserializeBlock(
						valueType,
						blockOffset,
						blockSize,
						out valueObject);

					return (syncResult, valueObject);
				}
				else
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return (false, valueObject);
				}
			}

			try
			{
				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, valueObject);
					}

					if (!loadVisitor.VisitLoad(
						asyncResult1.Item2,
						valueType,
						out valueObject))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						return (false, valueObject);
					}

					return (true, asyncResult1.Item2);
				}

				var asyncResult2 = await asyncBlockFormatSerializer
					.DeserializeBlockAsync(
						valueType,
						context,
						blockOffset,
						blockSize,
	
						asyncContext);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {valueType.Name}");

					return (false, valueObject);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return (false, default);
			}
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						return PopulateBlock<TValue>(
							value,
							blockOffset,
							blockSize);
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}
				}

				if (TryGetPopulateVisitor<TValue>(
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>();

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate<TValue>(
						value,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncBlockFormatSerializer.PopulateBlockAsync<TValue>(
					context,
					value,
					blockOffset,
					blockSize,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {nameof(TValue)}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> PopulateBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						return PopulateBlock(
							valueType,
							valueObject,
							blockOffset,
							blockSize);
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncBlockFormatSerializer.PopulateBlockAsync(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize,

					asyncContext))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK POPULATING FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		private bool TryGetDTO<TValue>(
			TValue value,
			out object dto)
		{
			dto = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>())
				return false;
				
			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			var visitable = value as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptSave(
					saveVisitor,
					ref dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
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

			return true;
		}

		private bool TryGetDTO(
			Type valueType,
			object valueObject,
			out object dto)
		{
			dto = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			var visitable = valueObject as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptSave(
					saveVisitor,
					ref dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}

			if (!saveVisitor.VisitSave(
				ref dto,
				valueType,
				valueObject))
			{
				logger?.LogError(
					GetType(),
					$"VISIT SAVE FAILED: {valueType.Name}");

				return false;
			}

			return true;
		}

		private bool TryGetLoadVisitor<TValue>(
			out ILoadVisitor loadVisitor)
		{
			loadVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>())
				return false;
			
			loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetLoadVisitor(
			Type valueType,
			out ILoadVisitor loadVisitor)
		{
			loadVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetPopulateVisitor<TValue>(
			out IPopulateVisitor populateVisitor)
		{
			populateVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>())
				return false;

			populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetPopulateVisitor(
			Type valueType,
			out IPopulateVisitor populateVisitor)
		{
			populateVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryPopulate<TValue>(
			TValue value,
			object dto,
			IPopulateVisitor populateVisitor)
		{
			var visitable = value as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptPopulate(
					populateVisitor,
					dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
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

			return true;
		}

		private bool TryPopulate(
			Type valueType,
			object valueObject,
			object dto,
			IPopulateVisitor populateVisitor)
		{
			var visitable = valueObject as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptPopulate(
					populateVisitor,
					dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}

			if (!populateVisitor.VisitPopulate(
				dto,
				valueType,
				valueObject))
			{
				logger?.LogError(
					GetType(),
					$"VISIT POPULATE FAILED: {nameof(valueType)}");

				return false;
			}

			return true;
		}
	}
}