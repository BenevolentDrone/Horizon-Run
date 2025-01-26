using System.Collections.Generic;

using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static class ObjectPoolAllocationCallbackFactory
    {
        public static PushToManagedPoolCallback<T> BuildPushToPoolCallback<T>(
            IManagedPool<T> root = null)
        {
            return new PushToManagedPoolCallback<T>
            {
                TargetPool = root
            };
        }
        
        public static PushToManagedPoolWhenAvailableCallback<T> BuildPushToManagedPoolWhenAvailableCallback<T>(
            IManagedPool<T> root = null)
        {
            return new PushToManagedPoolWhenAvailableCallback<T>(
                new List<IPoolElementFacade<T>>(),
                root);
        }
    }
}