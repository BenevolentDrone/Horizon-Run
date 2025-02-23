using System;

namespace HereticalSolutions.StanleyScript
{
    public class DefaultTypecasterToString
        : ITypecaster
    {
        private static readonly Type[] supportedSourceTypes = new Type[]
        {
            null
        };
        
        #region ITypecaster

        public Type TargetType => typeof(string);

        public Type[] SupportedSourceTypes => supportedSourceTypes;

        public bool WillCast<TSource>()
        {
            return true;
        }

        public bool WillCast(
            Type sourceType)
        {
            return true;
        }

        public bool TryCast<TSource, TValue>(
            TSource source,
            out TValue result)
        {
            if (typeof(TValue) != typeof(string))
            {
                result = default;

                return false;
            }

            string convertedValue;

            switch (source)
            {
                case StanleyHandle handle:
                    convertedValue = handle.Value.ToString();

                    break;

                default:
                    convertedValue = source.ToString();

                    break;
            }

            result = convertedValue.CastFromTo<string, TValue>();

            return true;
        }

        public bool TryCast(
            object source,
            out object result)
        {
            switch (source)
            {
                case StanleyHandle handle:
                    result = handle.Value.ToString();

                    break;

                default:
                    result = source.ToString();

                    break;
            }

            return true;
        }

        #endregion
    }
}