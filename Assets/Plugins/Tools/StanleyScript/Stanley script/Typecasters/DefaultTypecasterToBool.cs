using System;

namespace HereticalSolutions.StanleyScript
{
    public class DefaultTypecasterToBool
        : ITypecaster
    {
        //The list is composed ont the base of this reference:
        //https://learn.microsoft.com/ru-ru/dotnet/api/system.convert.toboolean?view=net-8.0
        private static readonly Type[] supportedSourceTypes = new[]
        {
            typeof(sbyte),
            typeof(ulong),
            typeof(uint),
            typeof(ushort),
            typeof(float),
            typeof(short),
            typeof(long),
            typeof(byte),
            typeof(char),
            typeof(DateTime),
            typeof(int),
            typeof(decimal),
            typeof(StanleyHandle),
            typeof(string)
        };
        
        #region ITypecaster

        public Type TargetType => typeof(bool);

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
            if (typeof(TValue) != typeof(bool))
            {
                result = default;

                return false;
            }

            bool convertedValue;
            
            switch (source)
            {
                case string stringValue:
                    convertedValue = Convert.ToBoolean(
                        stringValue);

                    break;

                case StanleyHandle handle:
                    convertedValue = Convert.ToBoolean(
                        handle.Value);

                    break;

                default:
                    convertedValue = Convert.ToBoolean(
                        source);

                    break;
            }

            result = convertedValue.CastFromTo<bool, TValue>();

            return true;
        }

        public bool TryCast(
            object source,
            out object result)
        {
            switch (source)
            {
                case string stringValue:
                    result = Convert.ToBoolean(
                        stringValue);

                    break;

                case StanleyHandle handle:
                    result = Convert.ToBoolean(
                        handle.Value);

                    break;

                default:
                    result = Convert.ToBoolean(
                        source);

                    break;
            }

            return true;
        }

        #endregion
    }
}