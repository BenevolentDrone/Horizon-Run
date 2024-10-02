using System.Collections.Generic;

namespace HereticalSolutions.Allocations.Factories
{
    public static class AllocationCallbackFactory
    {
        public static CompositeAllocationCallback<T> BuildCompositeCallback<T>(
            IAllocationCallback<T>[] callbacks)
        {
            List<IAllocationCallback<T>> callbacksList = new List<IAllocationCallback<T>>(callbacks);
            
            return new CompositeAllocationCallback<T>(callbacksList);
        }
    }
}