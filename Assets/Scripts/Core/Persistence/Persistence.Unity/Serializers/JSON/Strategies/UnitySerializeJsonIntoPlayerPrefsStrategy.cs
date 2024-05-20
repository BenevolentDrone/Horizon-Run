using HereticalSolutions.Persistence.Arguments;

using UnityEngine;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeJsonIntoPlayerPrefsStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            PlayerPrefs.SetString(prefsKey, json);
            
            PlayerPrefs.Save();
            
            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            json = string.Empty;

            if (!PlayerPrefs.HasKey(prefsKey))
                return false;
            
            json = PlayerPrefs.GetString(prefsKey);
            
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