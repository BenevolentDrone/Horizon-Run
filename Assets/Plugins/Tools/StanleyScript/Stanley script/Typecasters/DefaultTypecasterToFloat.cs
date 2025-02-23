using System;
using System.Globalization;

namespace HereticalSolutions.StanleyScript
{
    public class DefaultTypecasterToFloat
        : ITypecaster
    {
        //The list is composed ont the base of this reference:
        //https://learn.microsoft.com/en-us/dotnet/api/system.convert.tosingle?view=net-8.0
        private static readonly Type[] supportedSourceTypes = new[]
        {
            typeof(ulong),
            typeof(uint),
            typeof(ushort),
            typeof(float),
            typeof(sbyte),
            typeof(int),
            typeof(short),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(char),
            typeof(byte),
            typeof(bool),
            typeof(long),
            typeof(StanleyHandle),
            typeof(string)
        };
        
        #region ITypecaster

        public Type TargetType => typeof(float);

        public Type[] SupportedSourceTypes => supportedSourceTypes;

        public bool WillCast<TSource>()
        {
            return Array.IndexOf(supportedSourceTypes, typeof(TSource)) != -1;
        }

        public bool WillCast(
            Type sourceType)
        {
            return Array.IndexOf(supportedSourceTypes, sourceType) != -1;
        }
        
        public bool TryCast<TSource, TValue>(
            TSource source,
            out TValue result)
        {
            if (typeof(TValue) != typeof(float))
            {
                result = default;

                return false;
            }

            float convertedValue;

            switch (source)
            {
                case string stringValue:
                    convertedValue = float.Parse(
                        stringValue,
                        CultureInfo.InvariantCulture);

                    break;

                case StanleyHandle handle:
                    convertedValue = Convert.ToSingle(
                        handle.Value);

                    break;

                default:
                    convertedValue = Convert.ToSingle(
                        source);

                    break;
            }

            result = convertedValue.CastFromTo<float, TValue>();

            return true;
        }

        public bool TryCast(
            object source,
            out object result)
        {
            switch (source)
            {
                case string stringValue:
                    result = float.Parse(
                        stringValue,
                        CultureInfo.InvariantCulture);

                    break;

                case StanleyHandle handle:
                    result = Convert.ToSingle(
                        handle.Value);

                    break;

                default:
                    result = Convert.ToSingle(
                        source);

                    break;
            }

            return true;
        }

        #endregion
    }
}