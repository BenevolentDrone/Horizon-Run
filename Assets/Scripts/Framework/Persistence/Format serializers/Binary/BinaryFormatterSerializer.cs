using System;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class BinaryFormatterSerializer
        : IFormatSerializer
    {
        private readonly BinaryFormatter formatter;

        private readonly ILogger logger;

        public BinaryFormatterSerializer(
            BinaryFormatter formatter,
            ILogger logger)
        {
            this.formatter = formatter;

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

            formatter.Serialize(
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

            formatter.Serialize(
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

            var valueObject = formatter.Deserialize(
                strategyWithStream.Stream);

            value = valueObject.CastFromTo<object, TValue>();

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

            valueObject = formatter.Deserialize(
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

            var valueObject = formatter.Deserialize(
                strategyWithStream.Stream);

            value = valueObject.CastFromTo<object, TValue>();

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

            valueObject = formatter.Deserialize(
                strategyWithStream.Stream);

            return true;
        }

        #endregion
    }
}