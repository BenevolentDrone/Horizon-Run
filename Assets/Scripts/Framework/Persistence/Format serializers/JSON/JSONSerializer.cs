#if JSON_SUPPORT

using System;
using System.Collections.Generic;

using System.Linq;

using HereticalSolutions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class JSONSerializer
        : AFormatSerializer,
          IBlockFormatSerializer,
          IAsyncFormatSerializer,
          IAsyncBlockFormatSerializer
    {
        private readonly JsonSerializerSettings writeSerializerSettings;

        private readonly JsonSerializerSettings readSerializerSettings;

        public JSONSerializer(
            ILogger logger)
            : base(
                logger)
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
        }

        #region IFormatSerializer

        public override bool Serialize<TValue>(
            ISerializationCommandContext context,
            TValue value)
        {
            EnsureStrategyInitializedForSerialization(
                context);

            string json = JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                writeSerializerSettings);

            var result = TrySerialize<string>(
                context,
                json);

            EnsureStrategyFinalizedForSerialization(
                context);

            return result;
        }

        public override bool Serialize(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject)
        {
            EnsureStrategyInitializedForSerialization(
                context);

            string json = JsonConvert.SerializeObject(
                valueObject,
                Formatting.Indented,
                writeSerializerSettings);

            var result = TrySerialize<string>(
                context,
                json);

            EnsureStrategyFinalizedForSerialization(
                context);

            return result;
        }

        public override bool Deserialize<TValue>(
            ISerializationCommandContext context,
            out TValue value)
        {
            EnsureStrategyInitializedForDeserialization(
                context);

            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                value = default(TValue);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            value = JsonConvert.DeserializeObject<TValue>(
                json,
                readSerializerSettings);

            EnsureStrategyFinalizedForDeserialization(
                context);

            return true;
        }

        public override bool Deserialize(
            Type valueType,
            ISerializationCommandContext context,
            out object valueObject)
        {
            EnsureStrategyInitializedForDeserialization(
                context);

            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                valueObject = default(object);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            valueObject = JsonConvert.DeserializeObject(
                json,
                valueType,
                readSerializerSettings);

            EnsureStrategyFinalizedForDeserialization(
                context);

            return true;
        }

        public override bool Populate<TValue>(
            ISerializationCommandContext context,
            ref TValue value)
        {
            EnsureStrategyInitializedForDeserialization(
                context);

            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            JsonConvert.PopulateObject(
                json,
                value,
                readSerializerSettings);

            EnsureStrategyFinalizedForDeserialization(
                context);

            return true;
        }

        public override bool Populate(
            Type valueType,
            ISerializationCommandContext context,
            ref object valueObject)
        {
            EnsureStrategyInitializedForDeserialization(
                context);

            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            JsonConvert.PopulateObject(
                json,
                valueObject,
                readSerializerSettings);

            EnsureStrategyFinalizedForDeserialization(
                context);

            return true;
        }

        #endregion
    }
}

#endif