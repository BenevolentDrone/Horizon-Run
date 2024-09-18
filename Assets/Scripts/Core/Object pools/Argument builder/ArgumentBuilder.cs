using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a class that builds a chain of pool decorator arguments.
    /// </summary>
    public class ArgumentBuilder
    {
        private readonly List<IPoolPopArgument> argumentChain = new List<IPoolPopArgument>();

        /// <summary>
        /// Adds a pool decorator argument to the chain and assigns an instance of the argument to the specified out parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the pool decorator argument.</typeparam>
        /// <param name="instance">An out parameter that receives the instance of the pool decorator argument.</param>
        /// <returns>The instance of the ArgumentBuilder class.</returns>
        public ArgumentBuilder Add<TArgument>(out TArgument instance) where TArgument : IPoolPopArgument
        {
            instance = AllocationsFactory.ActivatorAllocationDelegate<TArgument>();

            argumentChain.Add(instance);

            return this;
        }

        /// <summary>
        /// Builds the array of pool decorator arguments.
        /// </summary>
        /// <returns>An array of IPoolPopArgument objects representing the built chain of arguments.</returns>
        public IPoolPopArgument[] Build()
        {
            return argumentChain.ToArray();
        }
    }
}