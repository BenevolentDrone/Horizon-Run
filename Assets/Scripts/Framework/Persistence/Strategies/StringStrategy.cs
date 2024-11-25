using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class StringStrategy
        : ISerializationStrategy
    {
        private static readonly Type[] allowedValueTypes = new Type[]
        {
            typeof(string)
        };

        private readonly ILogger logger;

        public string Value { get; private set; }

        public StringStrategy(
            ILogger logger = null)
        {
            this.logger = logger;

            Value = string.Empty;
        }

        #region ISerializationStrategy

        public Type[] AllowedValueTypes { get => allowedValueTypes; }

        #region Read

        public bool Read<TValue>(
            out TValue value)
        {
            AssertStrategyIsValid(
               typeof(TValue));

            value = Value.CastFromTo<string, TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertStrategyIsValid(
               valueType);

            value = Value.CastFromTo<string, object>();

            return true;
        }

        #endregion

        #region Write

        public bool Write<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
               typeof(TValue));

            Value = value.CastFromTo<TValue, string>();

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
               valueType);

            Value = value.CastFromTo<object, string>();

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            Value += value.CastFromTo<TValue, string>();

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType);

            Value += value.CastFromTo<object, string>();

            return true;
        }

        #endregion

        #endregion

        private void AssertStrategyIsValid(
            Type valueType)
        {
            if (valueType != typeof(string))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: {valueType.Name}"));
        }
    }
}