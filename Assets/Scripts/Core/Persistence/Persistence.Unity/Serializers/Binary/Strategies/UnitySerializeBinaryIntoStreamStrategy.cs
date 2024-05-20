using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeBinaryIntoStreamStrategy : IBinarySerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, BinaryFormatter formatter, object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out Stream fileStream))
                return false;
            
            formatter.Serialize(fileStream, value);
            
            UnityStreamIO.CloseStream(fileStream);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, BinaryFormatter formatter, out object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;

            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out Stream fileStream))
            {
                value = default(object);
                
                return false;
            }

            value = formatter.Deserialize(fileStream);
            
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