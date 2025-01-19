using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class StringStrategy
        : ISerializationStrategy,
          IStrategyWithFilter
    {
        private readonly ILogger logger;

        public string Value { get; set; }

        public StringStrategy(
            ILogger logger)
        {
            this.logger = logger;

            Value = string.Empty;
        }

        #region ISerializationStrategy

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

        #region IStrategyWithFilter

        public bool AllowsType<TValue>()
        {
            return typeof(TValue) == typeof(string);
        }

        public bool AllowsType(
            Type valueType)
        {
            return valueType == typeof(string);
        }

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