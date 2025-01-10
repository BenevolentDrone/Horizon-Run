using System;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class ObjectPoolAllocationCommandFactory
    {
        public static AllocationCommand<IPoolElementFacade<T>> BuildPoolElementFacadeAllocationCommand<T>(
            AllocationCommandDescriptor descriptor,
            Func<IPoolElementFacade<T>> poolElementAllocationDelegate,
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
        {
            return new AllocationCommand<IPoolElementFacade<T>>
            {
                Descriptor = descriptor,
                
                AllocationDelegate = poolElementAllocationDelegate,
                
                AllocationCallback = facadeAllocationCallback
            };
        }
    }
}