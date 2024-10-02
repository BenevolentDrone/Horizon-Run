using System;
using System.Collections;
using System.Globalization;
using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeCsvIntoTextFileStrategy : ICsvSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;

            string csv;
            
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
                
                csv = stringWriter.ToString();
            }
            
            return UnityTextFileIO.Write(fileSystemSettings, csv);
        }

        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;
            
            bool result = UnityTextFileIO.Read(fileSystemSettings, out string csv);

            if (!result)
            {
                value = default(object);
                
                return false;
            }

            using (StringReader stringReader = new StringReader(csv))
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
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityTextFileArgument)argument).Settings;
            
            UnityTextFileIO.Erase(fileSystemSettings);
        }
    }
}