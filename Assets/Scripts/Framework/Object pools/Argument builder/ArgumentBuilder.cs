using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.Pools.Factories
{
    public class ArgumentBuilder
    {
        private readonly List<IPoolPopArgument> argumentChain = new List<IPoolPopArgument>();

        public ArgumentBuilder Add<TArgument>(
            out TArgument instance)
            where TArgument : IPoolPopArgument
        {
            instance = AllocationsFactory.ActivatorAllocationDelegate<TArgument>();

            argumentChain.Add(instance);

            return this;
        }

        public IPoolPopArgument[] Build()
        {
            return argumentChain.ToArray();
        }
    }
}