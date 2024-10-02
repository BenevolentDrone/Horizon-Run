using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;

using UnityEngine;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeXmlIntoPlayerPrefsStrategy : IXmlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;

            string xml;
            
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                xml = stringWriter.ToString();
            }
            
            PlayerPrefs.SetString(prefsKey, xml);
            
            PlayerPrefs.Save();
            
            return true;
        }

        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;

            if (!PlayerPrefs.HasKey(prefsKey))
            {
                value = default(object);
                
                return false;
            }

            using (StringReader stringReader = new StringReader(PlayerPrefs.GetString(prefsKey)))
            {
                value = serializer.Deserialize(stringReader);
            }
            
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