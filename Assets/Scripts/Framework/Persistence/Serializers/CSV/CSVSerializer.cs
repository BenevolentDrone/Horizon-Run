using System;
using System.Collections.Generic;

using System.Linq;

using HereticalSolutions.Logging;

using CsvHelper;


namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class CSVSerializer
        : IFormatSerializer
    {
        private readonly bool includeHeader;

        private readonly ILogger logger;

        public JSONSerializer(
            bool includeHeader,
            ILogger logger = null)
        {
            this.includeHeader = includeHeader;

            this.logger = logger;
        }

        #region ISerializer

        public bool Serialize<TValue>(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            TValue DTO)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    DTO);

                return true;
            }
            
            string csv = SerializeToString<TValue>(
                DTO);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                csv);
        }

        public bool Serialize(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            Type DTOType,
            object DTO)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter(
                    textStreamStrategy,
                    DTOType,
                    DTO);

                return true;
            }

            string csv = SerializeToString(
                DTOType,
                DTO);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                csv);
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            out TValue DTO)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out DTO);
            }
            
            if (!PersistenceHelpers.Read<string>(
                strategy,
                arguments,
                out var csv))
            {
                DTO = default(object);

                return false;
            }

            return DeserializeFromString<TValue>(
                csv,
                out DTO);
        }

        public bool Deserialize(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            Type DTOType,
            out object DTO)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader(
                    textStreamStrategy,
                    DTOType,
                    out DTO);
            }

            if (!PersistenceHelpers.Read<string>(
                strategy,
                arguments,
                out var csv))
            {
                DTO = default(object);

                return false;
            }

            return DeserializeFromString(
                csv,
                DTOType,
                out DTO);
        }

        public bool Populate<TValue>(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            ref TValue value)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool Populate(
            ISerializationStrategy strategy,
            IReadOnlyObjectRepository arguments,
            Type ValueType,
            ref object valueObject)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

        private void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue DTO)
        {
            //This one is offered by CoPilot
            //using (var writer = new CsvWriter(textStreamStrategy.Stream))
            //{
            //    writer.WriteRecords(DTO);
            //}

            //This one is the one I had written some time ago
            var valueType = typeof(TValue);

            using (var csvWriter = new CsvWriter(
                textStreamStrategy.StreamWriter,
                CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader(valueType);

                    csvWriter.NextRecord();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    csvWriter.WriteRecords((IEnumerable)DTO);
                }
                else
                    csvWriter.WriteRecord(DTO);
            }
        }

        private void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object DTO)
        {
            //This one is offered by CoPilot
            //using (var writer = new CsvWriter(textStreamStrategy.Stream))
            //{
            //    writer.WriteRecords(DTO);
            //}

            //This one is the one I had written some time ago
            using (var csvWriter = new CsvWriter(
                textStreamStrategy.StreamWriter,
                CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader(valueType);

                    csvWriter.NextRecord();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    csvWriter.WriteRecords((IEnumerable)DTO);
                }
                else
                    csvWriter.WriteRecord(DTO);
            }
        }

        private bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue DTO)
        {
            DTO = default(TValue);

            var valueType = typeof(TValue);

            using (var csvReader = new CsvReader(
                textStreamStrategy.StreamReader,
                CultureInfo.InvariantCulture))
            {
                csvReader.Read();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    DTO = records;
                }
                else
                {
                    csvReader.Read();

                    DTO = csvReader.GetRecord(valueType);
                }
            }

            return true;
        }

        private bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object DTO)
        {
            DTO = default(object);

            using (var csvReader = new CsvReader(
                textStreamStrategy.StreamReader,
                CultureInfo.InvariantCulture))
            {
                csvReader.Read();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    DTO = records;
                }
                else
                {
                    csvReader.Read();

                    DTO = csvReader.GetRecord(valueType);
                }
            }

            return true;
        }

        private string SerializeToString<TValue>(
            TValue DTO)
        {
            string csv;

            var valueType = typeof(TValue);

            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(
                    stringWriter,
                    CultureInfo.InvariantCulture))
                {
                    if (includeHeader)
                    {
                        csvWriter.WriteHeader(valueType);

                        csvWriter.NextRecord();
                    }

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)DTO);
                    }
                    else
                        csvWriter.WriteRecord(DTO);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        private string SerializeToString(
            Type valueType,
            object DTO)
        {
            string csv;

            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(
                    stringWriter,
                    CultureInfo.InvariantCulture))
                {
                    if (includeHeader)
                    {
                        csvWriter.WriteHeader(valueType);

                        csvWriter.NextRecord();
                    }

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)DTO);
                    }
                    else
                        csvWriter.WriteRecord(DTO);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        private bool DeserializeFromString<TValue>(
            string csv,
            out TValue DTO)
        {
            DTO = default(TValue);

            var valueType = typeof(TValue);

            using (StringReader stringReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(
                    stringReader,
                    CultureInfo.InvariantCulture))
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

                        DTO = records;
                    }
                    else
                    {
                        csvReader.Read();

                        DTO = csvReader.GetRecord(valueType);
                    }
                }
            }

            return true;
        }

        private bool DeserializeFromString(
            string csv,
            Type valueType,
            out object DTO)
        {
            DTO = default(TValue);

            using (StringReader stringReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(
                    stringReader,
                    CultureInfo.InvariantCulture))
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

                        DTO = records;
                    }
                    else
                    {
                        csvReader.Read();

                        DTO = csvReader.GetRecord(valueType);
                    }
                }
            }

            return true;
        }
    }
}