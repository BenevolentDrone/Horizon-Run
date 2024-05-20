using System;
using System.Collections;
using System.Globalization;
using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeCsvIntoStreamStrategy : ICsvSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
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
            
            UnityStreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            UnityPersistentFilePathSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;

            value = default(object);
            
            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
                return false;
            
            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
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