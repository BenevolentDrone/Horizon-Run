using HereticalSolutions.Allocations;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    public class PushToManagedPoolCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        public IManagedPool<T> TargetPool { get; set; }

        public void OnAllocated(
            IPoolElementFacade<T> currentElementFacade)
        {
            currentElementFacade.Pool = TargetPool;
            
            TargetPool?.Push(currentElementFacade);
        }
    }
}