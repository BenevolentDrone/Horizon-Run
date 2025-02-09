#if XML_SUPPORT

using System;
using System.Text;
using System.IO;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

using System.Xml.Serialization;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class XMLSerializer
        : IFormatSerializer
    {
        private readonly ILogger logger;

        public XMLSerializer(
            ILogger logger)
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

            var xmlSerializer = new XmlSerializer(typeof(TValue));

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    xmlSerializer,
                    value);

                return true;
            }

            string xml = SerializeToString<TValue>(
                xmlSerializer,
                value);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                xml);
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

            var xmlSerializer = new XmlSerializer(valueType);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter(
                    textStreamStrategy,
                    xmlSerializer,
                    valueType,
                    valueObject);

                return true;
            }

            string xml = SerializeToString(
                xmlSerializer,
                valueType,
                valueObject);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                xml);
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            out TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            var xmlSerializer = new XmlSerializer(typeof(TValue));

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    xmlSerializer,
                    out value);
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string xml))
            {
                value = default(TValue);

                return false;
            }

            return DeserializeFromString<TValue>(
                xmlSerializer,
                xml,
                out value);
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

            var xmlSerializer = new XmlSerializer(valueType);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader(
                    textStreamStrategy,
                    xmlSerializer,
                    valueType,
                    out valueObject);
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string xml))
            {
                valueObject = default(object);

                return false;
            }

            return DeserializeFromString(
                xmlSerializer,
                xml,
                valueType,
                out valueObject);
        }

        public bool Populate<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            ref TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            var xmlSerializer = new XmlSerializer(typeof(TValue));

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    xmlSerializer,
                    out TValue newValue1);

                if (result)
                {
                    value = newValue1;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string xml))
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                xmlSerializer,
                xml,
                out TValue newValue2);

            if (result)
            {
                value = newValue2;
            }

            return result;
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

            var xmlSerializer = new XmlSerializer(valueType);

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
                    xmlSerializer,
                    valueType,
                    out var newValueObject1);

                if (result)
                {
                    valueObject = newValueObject1;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string xml))
            {
                return false;
            }

            result = DeserializeFromString(
                xmlSerializer,
                xml,
                valueType,
                out var newValueObject2);

            if (result)
            {
                valueObject = newValueObject2;
            }

            return result;
        }

        #endregion

        private void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            XmlSerializer xmlSerializer,
            TValue value)
        {
            xmlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                value);
        }

        private void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            XmlSerializer xmlSerializer,
            Type valueType,
            object valueObject)
        {
            xmlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                valueObject);
        }

        private bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            XmlSerializer xmlSerializer,
            out TValue value)
        {
            var valueObject = xmlSerializer.Deserialize(
                textStreamStrategy.StreamReader);

            value = valueObject.CastFromTo<object, TValue>();

            return true;
        }

        private bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            XmlSerializer xmlSerializer,
            Type valueType,
            out object valueObject)
        {
            valueObject = xmlSerializer.Deserialize(
                textStreamStrategy.StreamReader);

            return true;
        }

        private string SerializeToString<TValue>(
            XmlSerializer xmlSerializer,
            TValue value)
        {
            string xml;

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(
                    stringWriter,
                    value);

                xml = stringWriter.ToString();
            }

            return xml;
        }

        private string SerializeToString(
            XmlSerializer xmlSerializer,
            Type valueType,
            object valueObject)
        {
            string xml;

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(
                    stringWriter,
                    valueObject);

                xml = stringWriter.ToString();
            }

            return xml;
        }

        private bool DeserializeFromString<TValue>(
            XmlSerializer xmlSerializer,
            string xml,
            out TValue value)
        {
            object valueObject = default(object);

            using (StringReader stringReader = new StringReader(xml))
            {
                valueObject = xmlSerializer.Deserialize(stringReader);
            }

            value = valueObject.CastFromTo<object, TValue>();

            return true;
        }

        private bool DeserializeFromString(
            XmlSerializer xmlSerializer,
            string xml,
            Type valueType,
            out object valueObject)
        {
            using (StringReader stringReader = new StringReader(xml))
            {
                valueObject = xmlSerializer.Deserialize(stringReader);
            }

            return true;
        }
    }
}

#endif