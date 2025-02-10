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
        : ATextSerializer
    {
        public XMLSerializer(
            ILogger logger)
            : base(
                logger)
        {
        }

        protected override bool CanSerializeWithTextWriter => true;

        protected override void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(TValue));

            xmlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                value);
        }

        protected override void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
        {
            var xmlSerializer = new XmlSerializer(
                valueType);

            xmlSerializer.Serialize(
                textStreamStrategy.StreamWriter,
                valueObject);
        }

        protected override bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(TValue));

            var valueObject = xmlSerializer.Deserialize(
                textStreamStrategy.StreamReader);

            value = valueObject.CastFromTo<object, TValue>();

            return true;
        }

        protected override bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
        {
            var xmlSerializer = new XmlSerializer(
                valueType);

            valueObject = xmlSerializer.Deserialize(
                textStreamStrategy.StreamReader);

            return true;
        }

        protected override string SerializeToString<TValue>(
            TValue value)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(TValue));

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

        protected override string SerializeToString(
            Type valueType,
            object valueObject)
        {
            var xmlSerializer = new XmlSerializer(
                valueType);

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

        protected override bool DeserializeFromString<TValue>(
            string xml,
            out TValue value)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(TValue));

            object valueObject = default(object);

            using (StringReader stringReader = new StringReader(xml))
            {
                valueObject = xmlSerializer.Deserialize(stringReader);
            }

            value = valueObject.CastFromTo<object, TValue>();

            return true;
        }

        protected override bool DeserializeFromString(
            string xml,
            Type valueType,
            out object valueObject)
        {
            var xmlSerializer = new XmlSerializer(
                valueType);

            using (StringReader stringReader = new StringReader(xml))
            {
                valueObject = xmlSerializer.Deserialize(stringReader);
            }

            return true;
        }
    }
}

#endif