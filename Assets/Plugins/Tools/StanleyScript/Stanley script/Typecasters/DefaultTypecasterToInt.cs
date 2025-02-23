using System;

namespace HereticalSolutions.StanleyScript
{
    public class DefaultTypecasterToInt
        : ITypecaster
    {
        //The list is composed ont the base of this reference:
        //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions
        private static readonly Type[] supportedSourceTypes = new[]
        {
            typeof(uint),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(nint),
            typeof(nuint),
            typeof(StanleyHandle),
            typeof(string)
        };
        
        #region ITypecaster

        public Type TargetType => typeof(int);

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
            if (typeof(TValue) != typeof(int))
            {
                result = default;

                return false;
            }

            int convertedValue;
            
            switch (source)
            {
                case string stringValue:
                    convertedValue = int.Parse(
                        stringValue);

                    break;

                case StanleyHandle handle:
                    convertedValue = (int)handle.Value;

                    break;

                //All the cases allowed by CanHandle have explicit conversions
                //Yeah, we're doing implicit conversions here using explicit conversions. Sue me.
                //(btw this last bit was offered by Copilot and gave me a good laugh)
                default:
                    //convertedValue = (int)(object)source;
                    convertedValue = Convert.ToInt32(source);

                    break;
            }

            result = convertedValue.CastFromTo<int, TValue>();

            return true;
        }

        public bool TryCast(
            object source,
            out object result)
        {
            switch (source)
            {
                case string stringValue:
                    result = int.Parse(
                        stringValue);

                    break;

                case StanleyHandle handle:
                    result = (int)handle.Value;

                    break;

                //All the cases allowed by CanHandle have explicit conversions
                //Yeah, we're doing implicit conversions here using explicit conversions. Sue me.
                //(btw this last bit was offered by Copilot and gave me a good laugh)
                default:
                    //result = (int)source;
                    result = Convert.ToInt32(source);

                    break;
            }

            return true;
        }

        #endregion
    }
}