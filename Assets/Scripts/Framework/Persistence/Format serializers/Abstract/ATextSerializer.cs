using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    public abstract class ATextSerializer
		: AFormatSerializer,
          IBlockFormatSerializer,
          IAsyncFormatSerializer,
          IAsyncBlockFormatSerializer
    {
        public ATextSerializer(
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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    value);

                if (result)
                {
                    EnsureStrategyFinalizedForSerialization(
                        context);
    
                    return true;
                }
            }
            
            string stringValue = SerializeToString<TValue>(
                value);

            result = TrySerialize<string>(
                context,
                stringValue);

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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = SerializeWithTextWriter(
                    textStreamStrategy,
                    valueType,
                    valueObject);

                if (result)
                {
                    EnsureStrategyFinalizedForSerialization(
                        context);
    
                    return true;
                }
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = TrySerialize<string>(
                context,
                stringValue);

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

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out value);

                if (result)
                {
                    EnsureStrategyFinalizedForDeserialization(
                        context);
    
                    return result;
                }
            }
            
            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                value = default(TValue);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out valueObject);

                if (result)
                {
                    EnsureStrategyFinalizedForDeserialization(
                        context);
        
                    return result;
                }
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                valueObject = default(object);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
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

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
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

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = SerializeBlockWithTextWriter<TValue>(
                    textStreamStrategy,
                    value,
                    blockOffset,
                    blockSize);

                if (result)
                {
                    return result;
                }
            }

            string stringValue = SerializeToString<TValue>(
                value);

            result = TrySerializeBlock<string>(
                context,
                stringValue,
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

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = SerializeBlockWithTextWriter(
                    textStreamStrategy,
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

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = TrySerializeBlock<string>(
                context,
                stringValue,
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeBlockWithTextReader<TValue>(
                    textStreamStrategy,
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

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                value = default(TValue);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeBlockWithTextReader(
                    textStreamStrategy,
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

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                valueObject = default(object);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeBlockWithTextReader<TValue>(
                    textStreamStrategy,
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

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeBlockWithTextReader(
                    textStreamStrategy,
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

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

            if (CanSerializeWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await SerializeWithTextWriterAsync<TValue>(
                    textStreamStrategy,
                    value,

                    asyncContext);

                if (result)
                {
                    EnsureStrategyFinalizedForSerialization(
                        context);

                    return result;
                }
            }

            string stringValue = SerializeToString<TValue>(
                value);

            result = await TrySerializeAsync<string>(
                context,
                stringValue,

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

            if (CanSerializeWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await SerializeWithTextWriterAsync(
                    textStreamStrategy,
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

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = await TrySerializeAsync<string>(
                context,
                stringValue,

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

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await DeserializeWithTextReaderAsync<TValue>(
                    textStreamStrategy,

                    asyncContext);

                if (result.Item1)
                {
                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return result;
            }

            result.Item1 = DeserializeFromString<TValue>(
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

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await DeserializeWithTextReaderAsync(
                    textStreamStrategy,
                    valueType,

                    asyncContext);

                if (result.Item1)
                {
                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return result;
            }

            result.Item1 = DeserializeFromString(
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

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                var readerResult = await DeserializeWithTextReaderAsync<TValue>(
                    textStreamStrategy,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        value);

                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
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

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                var readerResult = await DeserializeWithTextReaderAsync(
                    textStreamStrategy,
                    valueType,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        valueObject);

                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
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

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await SerializeBlockWithTextWriterAsync<TValue>(
                    textStreamStrategy,
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

            string stringValue = SerializeToString<TValue>(
                value);

            result = await TrySerializeBlockAsync<string>(
                context,
                stringValue,
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

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await SerializeBlockWithTextWriterAsync(
                    textStreamStrategy,
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

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = await TrySerializeBlockAsync<string>(
                context,
                stringValue,
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await DeserializeBlockWithTextReaderAsync<TValue>(
                    textStreamStrategy,
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

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
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

            result.Item1 = DeserializeFromString<TValue>(
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = await DeserializeBlockWithTextReaderAsync(
                    textStreamStrategy,
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

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
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

            result.Item1 = DeserializeFromString(
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                var readerResult = await DeserializeBlockWithTextReaderAsync<TValue>(
                    textStreamStrategy,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        value);

                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
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

            result = DeserializeFromString<TValue>(
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

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                var readerResult = await DeserializeBlockWithTextReaderAsync(
                    textStreamStrategy,
                    valueType,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        valueObject);

                    EnsureStrategyFinalizedForDeserialization(
                        context);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
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

            result = DeserializeFromString(
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

        #region Serialize / deserialize with text reader / writer

        protected virtual bool CanSerializeWithTextWriter => false;

        protected virtual bool CanDeserializeWithTextReader => false;

        protected virtual bool CanSerializeBlockWithTextWriter => false;

        protected virtual bool CanDeserializeBlockWithTextReader => false;

        #region Regular

        protected virtual bool SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
		{
			throw new NotImplementedException();
		}

        #endregion

        #region Async

        protected virtual async Task<bool> SerializeWithTextWriterAsync<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return SerializeWithTextWriter<TValue>(
                textStreamStrategy,
                value);
        }

        protected virtual async Task<bool> SerializeWithTextWriterAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return SerializeWithTextWriter(
                textStreamStrategy,
                valueType,
                valueObject);
        }

        protected virtual async Task<(bool, TValue)> DeserializeWithTextReaderAsync<TValue>(
            TextStreamStrategy textStreamStrategy,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = DeserializeWithTextReader<TValue>(
                textStreamStrategy,
                out var value);

            return (result, value);
        }

        protected virtual async Task<(bool, object)> DeserializeWithTextReaderAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = DeserializeWithTextReader(
                textStreamStrategy,
                valueType,
                out var valueObject);

            return (result, valueObject);
        }

        #endregion

        #region Regular block

        protected virtual bool SerializeBlockWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value,
            int blockOffset,
            int blockSize)
        {
            throw new NotImplementedException();
        }

        protected virtual bool SerializeBlockWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject,
            int blockOffset,
            int blockSize)
        {
            throw new NotImplementedException();
        }

        protected virtual bool DeserializeBlockWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            int blockOffset,
            int blockSize,
            out TValue value)
        {
            throw new NotImplementedException();
        }

        protected virtual bool DeserializeBlockWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            int blockOffset,
            int blockSize,
            out object valueObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Async block

        protected virtual async Task<bool> SerializeBlockWithTextWriterAsync<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return SerializeBlockWithTextWriter<TValue>(
                textStreamStrategy,
                value,
                blockOffset,
                blockSize);
        }

        protected virtual async Task<bool> SerializeBlockWithTextWriterAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return SerializeBlockWithTextWriter(
                textStreamStrategy,
                valueType,
                valueObject,
                blockOffset,
                blockSize);
        }

        protected virtual async Task<(bool, TValue)> 
            DeserializeBlockWithTextReaderAsync<TValue>(
                TextStreamStrategy textStreamStrategy,
                int blockOffset,
                int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = DeserializeBlockWithTextReader<TValue>(
                textStreamStrategy,
                blockOffset,
                blockSize,
                out var value);

            return (result, value);
        }

        protected virtual async Task<(bool, object)> DeserializeBlockWithTextReaderAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = DeserializeBlockWithTextReader(
                textStreamStrategy,
                valueType,
                blockOffset,
                blockSize,
                out var valueObject);

            return (result, valueObject);
        }

        #endregion

        #endregion

        #region Serialize / deserialize to / from string

        protected abstract string SerializeToString<TValue>(
            TValue value);

		protected abstract string SerializeToString(
            Type valueType,
            object valueObject);

		protected abstract bool DeserializeFromString<TValue>(
            string stringValue,
            out TValue value);

		protected abstract bool DeserializeFromString(
            string stringValue,
            Type valueType,
            out object valueObject);

		#endregion
    }
}