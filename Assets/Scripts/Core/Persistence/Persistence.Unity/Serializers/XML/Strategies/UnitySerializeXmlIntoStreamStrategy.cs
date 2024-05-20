using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeXmlIntoStreamStrategy : IXmlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            serializer.Serialize(streamWriter, value);
            
            UnityStreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;

            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
            {
                value = default(object);

                return false;
            }

            value = serializer.Deserialize(streamReader);
            
            UnityStreamIO.CloseStream(streamReader);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            UnityStreamIO.Erase(fileSystemSettings);
        }
    }
}