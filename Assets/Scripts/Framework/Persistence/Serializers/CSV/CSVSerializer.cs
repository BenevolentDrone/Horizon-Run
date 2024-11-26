#define CSV_SUPPORT

#if CSV_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

using HereticalSolutions.Metadata;

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

        public CSVSerializer(
            bool includeHeader,
            ILogger logger = null)
        {
            this.includeHeader = includeHeader;

            this.logger = logger;
        }

        #region ISerializer

        public bool Serialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    value);

                return true;
            }
            
            string csv = SerializeToString<TValue>(
                value);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                csv);
        }

        public bool Serialize(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter(
                    textStreamStrategy,
                    valueType,
                    valueObject);

                return true;
            }

            string csv = SerializeToString(
                valueType,
                valueObject);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                csv);
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            out TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out value);
            }
            
            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string csv))
            {
                value = default(TValue);

                return false;
            }

            return DeserializeFromString<TValue>(
                csv,
                out value);
        }

        public bool Deserialize(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            out object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                return DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out valueObject);
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string csv))
            {
                valueObject = default(object);

                return false;
            }

            return DeserializeFromString(
                csv,
                valueType,
                out valueObject);
        }

        public bool Populate<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            ref TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out var newValue1);

                if (result)
                {
                    value = newValue1;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string csv))
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                csv,
                out var newValue2);

            if (result)
            {
                value = newValue2;
            }

            return result;
        }

        public bool Populate(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            ref object valueObject)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            bool result = false;

            if (strategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out var newValueObject1);

                if (result)
                {
                    valueObject = newValueObject1;
                }

                return result;
            }

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string csv))
            {
                return false;
            }

            result = DeserializeFromString(
                csv,
                valueType,
                out var newValueObject2);

            if (result)
            {
                valueObject = newValueObject2;
            }

            return result;
        }

        #endregion

        private void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
        {
            //This one is offered by CoPilot
            //using (var writer = new CsvWriter(textStreamStrategy.Stream))
            //{
            //    writer.WriteRecords(value);
            //}

            //This one is the one I had written some time ago
            var valueType = typeof(TValue);

            using (var csvWriter = new CsvWriter(
                textStreamStrategy.StreamWriter,
                CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader<TValue>();

                    csvWriter.NextRecord();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    csvWriter.WriteRecords((IEnumerable)value);
                }
                else
                    csvWriter.WriteRecord<TValue>(value);
            }
        }

        private void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
        {
            //This one is offered by CoPilot
            //using (var writer = new CsvWriter(textStreamStrategy.Stream))
            //{
            //    writer.WriteRecords(value);
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
                    csvWriter.WriteRecords((IEnumerable)valueObject);
                }
                else
                    csvWriter.WriteRecord(valueObject);
            }
        }

        private bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
        {
            value = default(TValue);

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

                    value = records.CastFromTo<IEnumerable<object>, TValue>();
                }
                else
                {
                    if (includeHeader)
                        csvReader.Read();

                    value = csvReader.GetRecord<TValue>();
                }
            }

            return true;
        }

        private bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
        {
            valueObject = default(object);

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

                    valueObject = records;
                }
                else
                {
                    if (includeHeader)
                        csvReader.Read();

                    valueObject = csvReader.GetRecord(valueType);
                }
            }

            return true;
        }

        private string SerializeToString<TValue>(
            TValue value)
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
                        csvWriter.WriteHeader<TValue>();

                        csvWriter.NextRecord();
                    }

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)value);
                    }
                    else
                        csvWriter.WriteRecord<TValue>(value);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        private string SerializeToString(
            Type valueType,
            object valueObject)
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
                        csvWriter.WriteRecords((IEnumerable)valueObject);
                    }
                    else
                        csvWriter.WriteRecord(valueObject);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        private bool DeserializeFromString<TValue>(
            string csv,
            out TValue value)
        {
            value = default(TValue);

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

                        value = records.CastFromTo<IEnumerable<object>, TValue>();
                    }
                    else
                    {
                        csvReader.Read();

                        value = csvReader.GetRecord<TValue>();
                    }
                }
            }

            return true;
        }

        private bool DeserializeFromString(
            string csv,
            Type valueType,
            out object valueObject)
        {
            valueObject = default(object);

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

                        valueObject = records;
                    }
                    else
                    {
                        csvReader.Read();

                        valueObject = csvReader.GetRecord(valueType);
                    }
                }
            }

            return true;
        }
    }
}
#endif