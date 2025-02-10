#if PROTOBUF_SUPPORT

using System;
using System.IO;

using HereticalSolutions.Logging;

using ProtoBuf;
using ProtobufInternalSerializer = ProtoBuf.Serializer;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class ProtobufSerializer
        : ABinarySerializer
    {
        public ProtobufSerializer(
            ILogger logger)
            : base(
                logger)
        {
        }

        protected override bool CanSerializeWithStream => true;

        protected override void SerializeWithStream<TValue>(
            IStrategyWithStream strategyWithStream,
            TValue value)
        {
            ProtobufInternalSerializer.Serialize<TValue>(
                strategyWithStream.Stream,
                value);
        }

        protected override void SerializeWithStream(
            IStrategyWithStream strategyWithStream,
            Type valueType,
            object valueObject)
        {
            ProtobufInternalSerializer.NonGeneric.Serialize(
                strategyWithStream.Stream,
                valueObject);
        }

        protected override bool DeserializeWithStream<TValue>(
            IStrategyWithStream strategyWithStream,
            out TValue value)
        {
            value = ProtobufInternalSerializer.Deserialize<TValue>(
                strategyWithStream.Stream);

            return true;
        }

        protected override bool DeserializeWithStream(
            IStrategyWithStream strategyWithStream,
            Type valueType,
            out object valueObject)
        {
            valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                valueType,
                strategyWithStream.Stream);

            return true;
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override byte[] SerializeToByteArray<TValue>(
            TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                ProtobufInternalSerializer.Serialize<TValue>(
                    memoryStream,
                    value);

                return memoryStream.ToArray();
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override byte[] SerializeToByteArray(
            Type valueType,
            object valueObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                ProtobufInternalSerializer.NonGeneric.Serialize(
                    memoryStream,
                    valueObject);

                return memoryStream.ToArray();
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override bool DeserializeFromByteArray<TValue>(
            byte[] byteArrayValue,
            out TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(
                    byteArrayValue,
                    0,
                    byteArrayValue.Length);

                memoryStream.Seek(
                    0,
                    SeekOrigin.Begin);

                value = ProtobufInternalSerializer.Deserialize<TValue>(
                    memoryStream);

                return true;
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override bool DeserializeFromByteArray(
            byte[] byteArrayValue,
            Type valueType,
            out object valueObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(
                    byteArrayValue,
                    0,
                    byteArrayValue.Length);

                memoryStream.Seek(
                    0,
                    SeekOrigin.Begin);

                valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                    valueType,
                    memoryStream);

                return true;
            }
        }
    }
}

#endif