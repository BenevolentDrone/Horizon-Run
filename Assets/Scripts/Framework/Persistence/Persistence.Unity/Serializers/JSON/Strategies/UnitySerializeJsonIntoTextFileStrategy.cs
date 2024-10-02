using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeJsonIntoTextFileStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;
            
            return UnityTextFileIO.Write(fileSystemSettings, json);
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;
            
            return UnityTextFileIO.Read(fileSystemSettings, out json);
        }
        
        public void Erase(ISerializationArgument argument)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;
            
            UnityTextFileIO.Erase(fileSystemSettings);
        }
    }
}