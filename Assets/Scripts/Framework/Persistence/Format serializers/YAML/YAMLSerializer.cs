#if YAML_SUPPORT

using System;
using System.Text;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

using YamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;
using YamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class YAMLSerializer
        : IFormatSerializer
    {
        private readonly YamlDotNetSerializer yamlSerializer;

        private readonly YamlDotNetDeserializer yamlDeserializer;

        private readonly ILogger logger;

        public YAMLSerializer(
            YamlDotNetSerializer yamlSerializer,
            YamlDotNetDeserializer yamlDeserializer,
            ILogger logger)
        {
            this.yamlSerializer = yamlSerializer;

            this.yamlDeserializer = yamlDeserializer;

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

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    value);

                return true;
            }

            string yaml = yamlSerializer.Serialize(
                value,
                typeof(TValue));

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                yaml);
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

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter(
                    textStreamStrategy,
                    valueType,
                    valueObject);

                return true;
            }

            string yaml = yamlSerializer.Serialize(
                valueObject,
                valueType);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                yaml);
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            out TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out value);
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string yaml))
            {
                value = default(TValue);

                return false;
            }

            value = yamlDeserializer.Deserialize<TValue>(yaml);

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

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out valueObject);
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string yaml))
            {
                valueObject = default(object);

                return false;
            }

            valueObject = yamlDeserializer.Deserialize(
                yaml,
                valueType);

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

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out var newValue);

                if (result)
                {
                    value = newValue;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string yaml))
            {
                return false;
            }

            value = yamlDeserializer.Deserialize<TValue>(yaml);

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

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out var newValueObject);

                if (result)
                {
                    valueObject = newValueObject;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string yaml))
            {
                return false;
            }

            valueObject = yamlDeserializer.Deserialize(
                yaml,
                valueType);

            return true;
        }

        #endregion

        private void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
        {
            yamlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                value,
                typeof(TValue));
        }

        private void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
        {
            yamlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                valueObject,
                valueType);
        }

        private bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
        {
            value = yamlDeserializer.Deserialize<TValue>(
                textStreamStrategy.StreamReader);

            return true;
        }

        private bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
        {
            valueObject = yamlDeserializer.Deserialize(
                textStreamStrategy.StreamReader,
                valueType);

            return true;
        }
    }
}

#endif