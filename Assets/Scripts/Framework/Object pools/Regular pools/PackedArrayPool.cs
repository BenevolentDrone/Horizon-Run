using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class PackedArrayPool<T>
        : IPool<T>,
	      IAllocationResizeable,
          ICleanuppable,
          IDisposable
    {
	    private readonly AllocationCommand<T> allocationCommand;
	    
        private readonly ILogger logger;

        private T[] packedArray;
        
        private int allocatedCount;

        public PackedArrayPool(
            T[] packedArray,
            AllocationCommand<T> allocationCommand,
            ILogger logger = null)
        {
            this.packedArray = packedArray;

            this.allocationCommand = allocationCommand;
            
            this.logger = logger;
            
            allocatedCount = 0;
        }
        
        #region IPool

        public T Pop()
        {
            T result = default;

            if (allocatedCount >= packedArray.Length)
            {
	            packedArray = PackedArrayPoolFactory.ResizePackedArrayPool(
		            packedArray,
		            allocationCommand,
		            logger);
            }
            
            result = packedArray[allocatedCount];
            
            allocatedCount++;

            return result;
        }
        
        public T Pop(IPoolPopArgument[] args)
        {
	        return Pop();
        }

        public void Push(T instance)
        {
            int lastAllocatedItemIndex = allocatedCount - 1;

            if (lastAllocatedItemIndex < 0)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO PUSH AN ITEM WHEN NO ITEMS ARE ALLOCATED");
                
                return;
            }

            int instanceIndex = Array.IndexOf(
                packedArray,
                instance);

            if (instanceIndex == -1)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO PUSH AN ITEM TO PACKED ARRAY IT DOES NOT BELONG TO");
                
                return;
            }

            if (instanceIndex > lastAllocatedItemIndex)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO PUSH AN ALREADY PUSHED ITEM: {instanceIndex}");
                
                return;
            }

            if (instanceIndex != lastAllocatedItemIndex)
            {
                //Swap pushed element and last allocated element
                var swap = packedArray[instanceIndex];

                packedArray[instanceIndex] = packedArray[lastAllocatedItemIndex];

                packedArray[lastAllocatedItemIndex] = swap;
            }

            allocatedCount--;
        }
        
        #endregion

        #region IAllocationResizeable

        public void Resize()
        {
            packedArray = PackedArrayPoolFactory.ResizePackedArrayPool(
                packedArray,
                allocationCommand,
                logger);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var item in packedArray)
                if (item is ICleanuppable)
                    (item as ICleanuppable).Cleanup();

            allocatedCount = 0;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var item in packedArray)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            for (int i = 0; i < packedArray.Length; i++)
            {
                packedArray[i] = default;
            }
            
            allocatedCount = 0;
        }

        #endregion
    }
}