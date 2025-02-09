#if JSON_SUPPORT

using System;
using System.Text;
using System.Collections.Generic;

using System.Linq;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class JSONSerializer
        : IFormatSerializer
    {
        private readonly JsonSerializerSettings writeSerializerSettings;

        private readonly JsonSerializerSettings readSerializerSettings;

        private readonly ILogger logger;

        public JSONSerializer(
            ILogger logger)
        {
            writeSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,   //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,                               //Tell me something... Why did I comment this out before?
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };

            writeSerializerSettings.Converters.Add(
                new Newtonsoft.Json.Converters.StringEnumConverter());

            readSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,                               //Tell me something... Why did I comment this out before?
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,   //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                MaxDepth = 10,

                //Courtesy of https://stackoverflow.com/questions/39383098/ignore-missing-types-during-deserialization-of-list
                SerializationBinder = new JsonSerializationBinder(
                    new DefaultSerializationBinder()),

                Error = (sender, args) =>
                {
                    if (args.CurrentObject == args.ErrorContext.OriginalObject
                        && args.ErrorContext.Error.InnerExceptionsAndSelf().OfType<JsonSerializationBinderException>().Any()
                        && args.ErrorContext.OriginalObject.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        logger?.LogError<JSONSerializer>(
                            $"EXCEPTION WAS THROWN DURING DESERIALIZATION: {args.ErrorContext.Error.Message}");

                        args.ErrorContext.Handled = true;
                    }
                }
            };

            this.logger = logger;
        }

        #region IFormatSerializer

        public bool Serialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
                strategy,
                arguments);

            string json = JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                writeSerializerSettings);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                json);
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

            string json = JsonConvert.SerializeObject(
                valueObject,
                Formatting.Indented,
                writeSerializerSettings);

            return PersistenceHelpers.TryWriteOrAppendPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetBytes,
                json);
        }

        public bool Deserialize<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            out TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string json))
            {
                value = default(TValue);

                return false;
            }

            value = JsonConvert.DeserializeObject<TValue>(
                json,
                readSerializerSettings);

            //DTO = (TValue)Activator.CreateInstance(typeof(TValue));
            //
            ////TODO: fix deserialize as <GENERIC> is NOT working at all - the value does NOT get populated properly
            ////WARNING: THIS ONE MAY NOT PROPERLY POPULATE. I HAVE NO IDEA WHY
            //JsonConvert.PopulateObject(
            //    json,
            //    DTO,
            //    readSerializerSettings);

            return true;
        }

        //TODO: fix deserialize as <GENERIC> is NOT working at all - the value does NOT get populated properly
        public bool Deserialize(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            Type valueType,
            out object valueObject)
        {
            //DTO = Activator.CreateInstance(DTOType);
            //
            //JsonConvert.PopulateObject(
            //    json,
            //    DTO,
            //    readSerializerSettings);

            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string json))
            {
                valueObject = default(object);

                return false;
            }

            valueObject = JsonConvert.DeserializeObject(
                json,
                valueType,
                readSerializerSettings);

            return true;
        }

        public bool Populate<TValue>(
            ISerializationStrategy strategy,
            IStronglyTypedMetadata arguments,
            ref TValue value)
        {
            PersistenceHelpers.EnsureStrategyInitializedForRead(
                strategy,
                arguments);

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                value,
                readSerializerSettings);

            return true;
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

            if (!PersistenceHelpers.TryReadPersistently<string>(
                strategy,
                arguments,
                Encoding.UTF8.GetString,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                valueObject,
                readSerializerSettings);

            return true;
        }

        #endregion
    }
}

#endif