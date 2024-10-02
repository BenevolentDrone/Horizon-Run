using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeYamlIntoStreamStrategy : IYamlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            streamWriter.Write(yaml);
            
            UnityStreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            yaml = string.Empty;
            
            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
                return false;
            
            yaml = streamReader.ReadToEnd();
            
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