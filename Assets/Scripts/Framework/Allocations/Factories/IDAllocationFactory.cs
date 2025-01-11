using System;

namespace HereticalSolutions.Allocations.Factories
{
    public static class IDAllocationFactory
    {
        public static TValue BuildID<TValue>()
        {
            //I'd be more happy with pattern matching but thanks anyway, copilot
            
            if (typeof(TValue) == typeof(Guid))
            {
                return (TValue)(object)BuildGUID();
            }
            else if (typeof(TValue) == typeof(int))
            {
                return (TValue)(object)BuildInt();
            }
            else
            {
                throw new ArgumentException("The type of ID is not supported.");
            }
        }

        public static Guid BuildGUID()
        {
            return Guid.NewGuid();
        }

        public static int BuildInt()
        {
            return new Random().Next();
        }

        public static ushort BuildUshort()
        {
            return Convert.ToUInt16(
                new Random().Next(
                    ushort.MinValue,
                    ushort.MaxValue));
        }
    }
}