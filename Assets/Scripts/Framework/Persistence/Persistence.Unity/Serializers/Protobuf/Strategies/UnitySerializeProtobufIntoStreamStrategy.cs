using System;
using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using ProtoBuf;
using ProtobufInternalSerializer = ProtoBuf.Serializer;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeProtobufIntoStreamStrategy : IProtobufSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out Stream fileStream))
                return false;
            
            ProtobufInternalSerializer.Serialize(fileStream, value);
            
            //NOT WORKING
            //https://stackoverflow.com/questions/10510081/protobuf-net-argumentnullexception
            //ProtobufInternalSerializer.NonGeneric.SerializeWithLengthPrefix(fileStream, value, PrefixStyle.Base128, 1);
            
            UnityStreamIO.CloseStream(fileStream);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;

            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out Stream fileStream))
            {
                value = default(object);
                
                return false;
            }

            value = ProtobufInternalSerializer.Deserialize(valueType, fileStream);
            
            //NOT WORKING
            //value = ProtobufInternalSerializer.NonGeneric.Deserialize(valueType, fileStream);
            
            UnityStreamIO.CloseStream(fileStream);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            UnityStreamIO.Erase(fileSystemSettings);
        }
    }
}