#if CSV_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

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

        protected override void SerializeWithTextWriter<TValue>(
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
        }

        protected override void SerializeWithTextWriter(
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