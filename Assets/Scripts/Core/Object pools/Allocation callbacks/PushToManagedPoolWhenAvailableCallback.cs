using System.Collections.Generic;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    public class PushToManagedPoolWhenAvailableCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        private readonly List<IPoolElementFacade<T>> elementsToPush;

        private IManagedPool<T> targetPool;
        
        public PushToManagedPoolWhenAvailableCallback(
            List<IPoolElementFacade<T>> elementsToPush,
            IManagedPool<T> targetPool = null)
        {
            this.elementsToPush = elementsToPush;

            this.targetPool = targetPool;
        }

        public IManagedPool<T> TargetPool
        {
            get => targetPool;
            set
            {
                targetPool = value;

                if (targetPool != null)
                {
                    foreach (var element in elementsToPush)
                    {
                        element.Pool = targetPool;
                        
                        targetPool.Push(element);
                    }

                    elementsToPush.Clear();
                }
            }
        }

        public void OnAllocated(
            IPoolElementFacade<T> currentElementFacade)
        {
            if (targetPool != null)
            {
                currentElementFacade.Pool = targetPool;
                
                targetPool.Push(currentElementFacade);
            }
            else
            {
                elementsToPush.Add(currentElementFacade);
            }
        }
    }
}