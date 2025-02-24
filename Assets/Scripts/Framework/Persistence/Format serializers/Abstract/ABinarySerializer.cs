using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ABinarySerializer
		: AFormatSerializer,
		  IBlockFormatSerializer,
		  IAsyncFormatSerializer,
		  IAsyncBlockFormatSerializer
	{
		public ABinarySerializer(
			ILogger logger)
			: base(
				logger)
		{
		}

		#region IFormatSerializer

		public override bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = SerializeWithStream<TValue>(
					strategyWithStream,
					value);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);
	
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = SerializeWithStream(
					strategyWithStream,
					valueType,
					valueObject);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);
	
					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream<TValue>(
					strategyWithStream,
					out value);

				if (result)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);
	
					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				value = default(TValue);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out value);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream(
					strategyWithStream,
					valueType,
					out valueObject);

				if (result)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);
	
					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				valueObject = default(object);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out valueObject);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Populate<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream<TValue>(
					strategyWithStream,
					out var newValue1);

				if (result)
				{
					PopulateWithReflection(
						newValue1,
						value);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out var newValue2);

			if (result)
			{
				PopulateWithReflection(
					newValue2,
					value);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream(
					strategyWithStream,
					valueType,
					out var newValueObject1);

				if (result)
				{
					PopulateWithReflection(
						newValueObject1,
						valueObject);

					EnsureStrategyFinalizedForDeserialization(
						context);
	
					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out var newValueObject2);

			if (result)
			{
				PopulateWithReflection(
					newValueObject2,
					valueObject);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		#endregion

		#region IBlockFormatSerializer

		public bool SerializeBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			bool result = false;

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = SerializeBlockWithStream<TValue>(
					strategyWithStream,
					value,
					blockOffset,
					blockSize);

				if (result)
				{
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = TrySerializeBlock<byte[]>(
				context,
				byteArrayValue,
				blockOffset,
				blockSize);

			return result;
		}

		public bool SerializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = SerializeBlockWithStream(
					strategyWithStream,
					valueType,
					valueObject,
					blockOffset,
					blockSize);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);

					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = TrySerializeBlock<byte[]>(
				context,
				byteArrayValue,
				blockOffset,
				blockSize);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public bool DeserializeBlock<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeBlockWithStream<TValue>(
					strategyWithStream,
					blockOffset,
					blockSize,
					out value);

				if (result)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				value = default(TValue);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out value);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public bool DeserializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeBlockWithStream(
					strategyWithStream,
					valueType,
					blockOffset,
					blockSize,
					out valueObject);

				if (result)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				valueObject = default(object);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out valueObject);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public bool PopulateBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeBlockWithStream<TValue>(
					strategyWithStream,
					blockOffset,
					blockSize,
					out var newValue1);

				if (result)
				{
					PopulateWithReflection(
						newValue1,
						value);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out var newValue2);

			if (result)
			{
				PopulateWithReflection(
					newValue2,
					value);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public bool PopulateBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeBlockWithStream(
					strategyWithStream,
					valueType,
					blockOffset,
					blockSize,
					out var newValueObject1);

				if (result)
				{
					PopulateWithReflection(
						newValueObject1,
						valueObject);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out var newValueObject2);

			if (result)
			{
				PopulateWithReflection(
					newValueObject2,
					valueObject);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		#endregion

		#region IAsyncFormatSerializer

		public override async Task<bool> SerializeAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await SerializeWithStreamAsync<TValue>(
					strategyWithStream,
					value,
					
					asyncContext);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);

					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = await TrySerializeAsync<byte[]>(
				context,
				byteArrayValue,
				
				asyncContext);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override async Task<bool> SerializeAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await SerializeWithStreamAsync(
					strategyWithStream,
					valueType,
					valueObject,
					
					asyncContext);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);

					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = await TrySerializeAsync<byte[]>(
				context,
				byteArrayValue,
				
				asyncContext);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override async Task<(bool, TValue)> DeserializeAsync<TValue>(
			ISerializationCommandContext context,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			(bool, TValue) result = (false, default(TValue));

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await DeserializeWithStreamAsync<TValue>(
					strategyWithStream,

					asyncContext);

				if (result.Item1)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			result.Item1 = DeserializeFromByteArray<TValue>(
				byteArrayResult.Item2,
				out result.Item2);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override async Task<(bool, object)> DeserializeAsync(
			Type valueType,
			ISerializationCommandContext context,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			(bool, object) result = (false, default(object));

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await DeserializeWithStreamAsync(
					strategyWithStream,
					valueType,
					
					asyncContext);

				if (result.Item1)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			result.Item1 = DeserializeFromByteArray(
				byteArrayResult.Item2,
				valueType,
				out result.Item2);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override async Task<bool> PopulateAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				var streamResult = await DeserializeWithStreamAsync<TValue>(
					strategyWithStream,
					
					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						value);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayResult.Item2,
				out var newValue);

			if (result)
			{
				PopulateWithReflection(
					newValue,
					value);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override async Task<bool> PopulateAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				var streamResult = await DeserializeWithStreamAsync(
					strategyWithStream,
					valueType,
					
					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						valueObject);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayResult.Item2,
				valueType,
				out var newValue);

			if (result)
			{
				PopulateWithReflection(
					newValue,
					valueObject);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		#endregion

		#region IAsyncBlockFormatSerializer

		public async Task<bool> SerializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await SerializeBlockWithStreamAsync<TValue>(
					strategyWithStream,
					value,
					blockOffset,
					blockSize,

					asyncContext);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);

					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = await TrySerializeBlockAsync<byte[]>(
				context,
				byteArrayValue,
				blockOffset,
				blockSize,

				asyncContext);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public async Task<bool> SerializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			bool result = false;

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await SerializeBlockWithStreamAsync(
					strategyWithStream,
					valueType,
					valueObject,
					blockOffset,
					blockSize,

					asyncContext);

				if (result)
				{
					EnsureStrategyFinalizedForSerialization(
						context);

					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = await TrySerializeBlockAsync<byte[]>(
				context,
				byteArrayValue,
				blockOffset,
				blockSize,

				asyncContext);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			(bool, TValue) result = (false, default(TValue));

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await DeserializeBlockWithStreamAsync<TValue>(
					strategyWithStream,
					blockOffset,
					blockSize,

					asyncContext);

				if (result.Item1)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
					blockOffset,
					blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			result.Item1 = DeserializeFromByteArray<TValue>(
				byteArrayResult.Item2,
				out result.Item2);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public async Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			(bool, object) result = (false, default(object));

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = await DeserializeBlockWithStreamAsync(
					strategyWithStream,
					valueType,
					blockOffset,
					blockSize,

					asyncContext);

				if (result.Item1)
				{
					EnsureStrategyFinalizedForDeserialization(
						context);

					return result;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			result.Item1 = DeserializeFromByteArray(
				byteArrayResult.Item2,
				valueType,
				out result.Item2);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public async Task<bool> PopulateBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				var streamResult = await DeserializeBlockWithStreamAsync<TValue>(
					strategyWithStream,
					blockOffset,
					blockSize,

					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						value);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayResult.Item2,
				out var newValue);

			if (result)
			{
				PopulateWithReflection(
					newValue,
					value);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public async Task<bool> PopulateBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				var streamResult = await DeserializeBlockWithStreamAsync(
					strategyWithStream,
					valueType,
					blockOffset,
					blockSize,

					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						valueObject);

					EnsureStrategyFinalizedForDeserialization(
						context);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayResult.Item2,
				valueType,
				out var newValue);

			if (result)
			{
				PopulateWithReflection(
					newValue,
					valueObject);
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		#endregion

		#region Serialize / deserialize with stream

		protected virtual bool CanSerializeWithStream => false;

		protected virtual bool CanDeserializeWithStream => false;

		protected virtual bool CanSerializeBlockWithStream => false;

		protected virtual bool CanDeserializeBlockWithStream => false;

		#region Regular

		protected virtual bool SerializeWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Async

		protected virtual async Task<bool> SerializeWithStreamAsync<TValue>(
			IStrategyWithStream strategyWithStream,
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return SerializeWithStream<TValue>(
				strategyWithStream,
				value);
		}

		protected virtual async Task<bool> SerializeWithStreamAsync(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return SerializeWithStream(
				strategyWithStream,
				valueType,
				valueObject);
		}

		protected virtual async Task<(bool, TValue)> DeserializeWithStreamAsync<TValue>(
			IStrategyWithStream strategyWithStream,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool result = DeserializeWithStream<TValue>(
				strategyWithStream,
				out var value);

			return (result, value);
		}

		protected virtual async Task<(bool, object)> DeserializeWithStreamAsync(
			IStrategyWithStream strategyWithStream,
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool result = DeserializeWithStream(
				strategyWithStream,
				valueType,
				out var valueObject);

			return (result, valueObject);
		}

		#endregion

		#region Regular block

		protected virtual bool SerializeBlockWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeBlockWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeBlockWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeBlockWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Async block

		protected virtual async Task<bool> SerializeBlockWithStreamAsync<TValue>(
			IStrategyWithStream strategyWithStream,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return SerializeBlockWithStream<TValue>(
				strategyWithStream,
				value,
				blockOffset,
				blockSize);
		}

		protected virtual async Task<bool> SerializeBlockWithStreamAsync(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return SerializeBlockWithStream(
				strategyWithStream,
				valueType,
				valueObject,
				blockOffset,
				blockSize);
		}

		protected virtual async Task<(bool, TValue)> DeserializeBlockWithStreamAsync<TValue>(
			IStrategyWithStream strategyWithStream,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool result = DeserializeBlockWithStream<TValue>(
				strategyWithStream,
				blockOffset,
				blockSize,
				out var value);

			return (result, value);
		}

		protected virtual async Task<(bool, object)> DeserializeBlockWithStreamAsync(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool result = DeserializeBlockWithStream(
				strategyWithStream,
				valueType,
				blockOffset,
				blockSize,
				out var valueObject);

			return (result, valueObject);
		}

		#endregion

		#endregion

		#region Serialize / deserialize to / from byte array

		protected abstract bool DeserializeFromByteArray<TValue>(
			byte[] byteArrayValue,
			out TValue value);

		protected abstract bool DeserializeFromByteArray(
			byte[] byteArrayValue,
			Type valueType,
			out object valueObject);

		protected abstract byte[] SerializeToByteArray<TValue>(
			TValue value);

		protected abstract byte[] SerializeToByteArray(
			Type valueType,
			object valueObject);

		#endregion
	}
}