using System;
using System.Collections;
using System.Globalization;
using System.IO;

using HereticalSolutions.Persistence.Arguments;

using CsvHelper;

using UnityEngine;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeCsvIntoPlayerPrefsStrategy : ICsvSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;
            
            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(stringWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader(valueType);
                    
                    csvWriter.NextRecord();
                    
                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)value);
                    }
                    else
                        csvWriter.WriteRecord(value);
                }
                
                PlayerPrefs.SetString(prefsKey, stringWriter.ToString());
            }
            
            PlayerPrefs.Save();
            
            return true;
        }

        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            string prefsKey = ((UnityPlayerPrefsArgument)argument).PrefsKey;

            if (!PlayerPrefs.HasKey(prefsKey))
            {
                value = default(object);
                
                return false;
            }

            using (StringReader stringReader = new StringReader(PlayerPrefs.GetString(prefsKey)))
            {
                using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Read();
                
                    csvReader.ReadHeader();
                    
                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                            ? valueType.GetGenericArrayUnderlyingType()
                            : valueType.GetGenericEnumerableUnderlyingType();

                        var records = csvReader.GetRecords(underlyingType);

                        value = records;
                    }
                    else
                    {
                        csvReader.Read();   
                    
                        value = csvReader.GetRecord(valueType);
                    }
                }
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