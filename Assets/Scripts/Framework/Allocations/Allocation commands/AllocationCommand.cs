using System;

namespace HereticalSolutions.Allocations
{
    public class AllocationCommand<T>
    {
        public AllocationCommandDescriptor Descriptor { get; set; }

        public Func<T> AllocationDelegate { get; set; }

        public IAllocationCallback<T> AllocationCallback { get; set; }
    }
}