using HereticalSolutions.Persistence.Arguments;

using UnityEngine;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeYamlIntoPlayerPrefsStrategy : IYamlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            PlayerPrefs.SetString(prefsKey, yaml);
            
            PlayerPrefs.Save();
            
            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            yaml = string.Empty;

            if (!PlayerPrefs.HasKey(prefsKey))
                return false;
            
            yaml = PlayerPrefs.GetString(prefsKey);
            
            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            if (!PlayerPrefs.HasKey(prefsKey))
                return;
            
            PlayerPrefs.DeleteKey(prefsKey);
        }
    }
}