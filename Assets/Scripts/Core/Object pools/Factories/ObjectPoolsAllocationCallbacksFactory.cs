using System.Collections.Generic;

using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory class for creating object pool allocation callbacks.
    /// </summary>
    public static class ObjectPoolsAllocationCallbacksFactory
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