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
        : ATextSerializer
    {
        private readonly YamlDotNetSerializer yamlSerializer;

        private readonly YamlDotNetDeserializer yamlDeserializer;

        public YAMLSerializer(
            YamlDotNetSerializer yamlSerializer,
            YamlDotNetDeserializer yamlDeserializer,
            ILogger logger)
            : base(
                logger)
        {
            this.yamlSerializer = yamlSerializer;

            this.yamlDeserializer = yamlDeserializer;
        }

        protected override bool CanSerializeWithTextWriter => true;

        protected override bool CanDeserializeWithTextReader => true;

        protected override bool SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
        {
            yamlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                value,
                typeof(TValue));

            return true;
        }

        protected override bool SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
        {
            yamlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                valueObject,
                valueType);

            return true;
        }

        protected override bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
        {
            value = yamlDeserializer.Deserialize<TValue>(
                textStreamStrategy.StreamReader);

            return true;
        }

        protected override bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
        {
            valueObject = yamlDeserializer.Deserialize(
                textStreamStrategy.StreamReader,
                valueType);

            return true;
        }

        protected override string SerializeToString<TValue>(
            TValue value)
        {
            return yamlSerializer.Serialize(
                value,
                typeof(TValue));
        }

        protected override string SerializeToString(
            Type valueType,
            object valueObject)
        {
            return yamlSerializer.Serialize(
                valueObject,
                valueType);
        }

        protected override bool DeserializeFromString<TValue>(
            string stringValue,
            out TValue value)
        {
            value = yamlDeserializer.Deserialize<TValue>(
                stringValue);

            return true;
        }

        protected override bool DeserializeFromString(
            string stringValue,
            Type valueType,
            out object valueObject)
        {
            valueObject = yamlDeserializer.Deserialize(
                stringValue,
                valueType);

            return true;
        }
    }
}

#endif