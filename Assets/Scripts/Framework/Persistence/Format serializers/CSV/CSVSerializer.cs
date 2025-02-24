#if CSV_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

using CsvHelper;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class CSVSerializer
        : ATextSerializer
    {
        private readonly bool includeHeader;

        public CSVSerializer(
            bool includeHeader,
            ILogger logger)
            : base(
                logger)
        {
            this.includeHeader = includeHeader;
        }

        protected override bool CanSerializeWithTextWriter => true;

        protected override bool CanDeserializeWithTextReader => true;

        protected override bool SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
        {
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

            return true;
        }

        protected override bool SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
        {
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

            return true;
        }

        protected override bool DeserializeWithTextReader<TValue>(
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

        protected override bool DeserializeWithTextReader(
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

        protected virtual async Task<bool> SerializeWithTextWriterAsync<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var valueType = typeof(TValue);

            using (var csvWriter = new CsvWriter(
                textStreamStrategy.StreamWriter,
                CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader<TValue>();

                    await csvWriter.NextRecordAsync();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    await csvWriter.WriteRecordsAsync((IEnumerable)value);
                }
                else
                    csvWriter.WriteRecord<TValue>(value);
            }

            return true;
        }

        protected virtual async Task<bool> SerializeWithTextWriterAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            using (var csvWriter = new CsvWriter(
                textStreamStrategy.StreamWriter,
                CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader(valueType);

                    await csvWriter.NextRecordAsync();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    await csvWriter.WriteRecordsAsync((IEnumerable)valueObject);
                }
                else
                    csvWriter.WriteRecord(valueObject);
            }

            return true;
        }

        protected virtual async Task<(bool, TValue)> DeserializeWithTextReaderAsync<TValue>(
            TextStreamStrategy textStreamStrategy,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var value = default(TValue);

            var valueType = typeof(TValue);

            using (var csvReader = new CsvReader(
                textStreamStrategy.StreamReader,
                CultureInfo.InvariantCulture))
            {
                await csvReader.ReadAsync();

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
                        await csvReader.ReadAsync();

                    value = csvReader.GetRecord<TValue>();
                }
            }

            return (true, value);
        }

        protected virtual async Task<(bool, object)> DeserializeWithTextReaderAsync(
            TextStreamStrategy textStreamStrategy,
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var valueObject = default(object);

            using (var csvReader = new CsvReader(
                textStreamStrategy.StreamReader,
                CultureInfo.InvariantCulture))
            {
                await csvReader.ReadAsync();

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
                        await csvReader.ReadAsync();

                    valueObject = csvReader.GetRecord(valueType);
                }
            }

            return (true, valueObject);
        }

        protected override string SerializeToString<TValue>(
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

        protected override string SerializeToString(
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

        protected override bool DeserializeFromString<TValue>(
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

        protected override bool DeserializeFromString(
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