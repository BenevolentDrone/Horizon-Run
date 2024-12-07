#define PROTOBUF_SUPPORT

#if PROTOBUF_SUPPORT
using System;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

using ProtoBuf;
using ProtobufInternalSerializer = ProtoBuf.Serializer;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class ProtobufSerializer
        : IFormatSerializer
    {
        private readonly ILogger logger;

        public ProtobufSerializer(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        #region IFormatSerializer

        public bool Serialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                return false;
            }

            ProtobufInternalSerializer.Serialize<TValue>(
                strategyWithStream.Stream,
                value);

            return true;
        }

        public bool Serialize(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                return false;
            }

            ProtobufInternalSerializer.NonGeneric.Serialize(
                strategyWithStream.Stream,
                valueObject);

            return true;
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            out TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                value = default;

                return false;
            }

            value = ProtobufInternalSerializer.Deserialize<TValue>(
                strategyWithStream.Stream);

            return true;
        }

        public bool Deserialize(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            out object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                valueObject = default;

                return false;
            }

            valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                valueType,
                strategyWithStream.Stream);

            return true;
        }

        public bool Populate<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            ref TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                return false;
            }

            value = ProtobufInternalSerializer.Deserialize<TValue>(
                strategyWithStream.Stream,
                value);

            return true;
        }

        public bool Populate(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            ref object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is not IStrategyWithStream strategyWithStream)
            {
                logger?.LogError(
                    GetType(),
                    $"{GetType().Name} ONLY SUPPORTS STRATEGIES WITH STREAMS");

                return false;
            }

            valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                valueType,
                strategyWithStream.Stream,
                valueObject);

            return true;
        }

        #endregion
    }
}
#endif