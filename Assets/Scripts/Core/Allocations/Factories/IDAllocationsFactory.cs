using System;

namespace HereticalSolutions.Allocations.Factories
{
    /// <summary>
    /// A factory for generating ID allocations.
    /// </summary>
    public static class IDAllocationsFactory
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

        /// <summary>
        /// Builds a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        public static Guid BuildGUID()
        {
            // Generate a new GUID and return it
            return Guid.NewGuid();
        }

        /// <summary>
        /// Builds a random integer.
        /// </summary>
        /// <returns>A randomly generated integer.</returns>
        public static int BuildInt()
        {
            // Generate a random integer and return it
            return new Random().Next();
        }
    }
}