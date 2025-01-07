using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class AppendablePackedArrayManagedPool<T> :
        PackedArrayManagedPool<T>
    {
        private readonly AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand;
        
        private readonly AllocationCommand<T> nullValueAllocationCommand;
        
        public AppendablePackedArrayManagedPool(
            IPoolElementFacade<T>[] packedArray,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            
            AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand,
            AllocationCommand<T> nullValueAllocationCommand,
            
            ILogger logger = null)
            : base (
                packedArray,
                facadeAllocationCommand,
                valueAllocationCommand,
                true,
                logger)
        {
            this.appendFacadeAllocationCommand = appendFacadeAllocationCommand;
            
            this.nullValueAllocationCommand = nullValueAllocationCommand;
        }
        
        public override IPoolElementFacade<T> Pop(IPoolPopArgument[] args)
        {
            #region Append from argument
            
            if (args.TryGetArgument<AppendToPoolArgument>(out var arg))
            {
                logger?.Log(
                    GetType(),
                    "APPEND ARGUMENT RECEIVED, APPENDING");

                int firstNonAllocatedItemIndex = allocatedCount;
                
                //Find first uninitialized item

                int firstUninitializedItemIndex = allocatedCount;

                while (packedArray[firstUninitializedItemIndex].Status != EPoolElementStatus.UNINITIALIZED)
                {
                    firstUninitializedItemIndex++;

                    if (firstUninitializedItemIndex >= packedArray.Length)
                    {
                        packedArray = PackedArrayPoolFactory.ResizePackedArrayManagedPool<T>(
                            packedArray,
                            appendFacadeAllocationCommand,
                            nullValueAllocationCommand,
                            logger);
                    }
                }
                
                //Swap

                if (firstUninitializedItemIndex != firstNonAllocatedItemIndex)
                {
                    //Swap the current element with the first non-allocated item

                    var swap = packedArray[firstNonAllocatedItemIndex];

                    packedArray[firstNonAllocatedItemIndex] = packedArray[firstUninitializedItemIndex];

                    packedArray[firstUninitializedItemIndex] = swap;
                }
                
                int index = allocatedCount;
	        
                var result = packedArray[index];
            
                allocatedCount++;
	        
                //Update metadata

                IPoolElementFacadeWithMetadata<T> resultWithMetadata = result as IPoolElementFacadeWithMetadata<T>;

                if (resultWithMetadata == null)
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            "PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
                }

                //Update index
                
                var indexedFacade = resultWithMetadata as IIndexed;

                if (indexedFacade == null)
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            "PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
                }

                indexedFacade.Index = index;

                //Validate pool

                if (result.Pool == null)
                {
                    result.Pool = this;
                }
                
                //Update facade
                
                result.Status = EPoolElementStatus.UNINITIALIZED;
            
                return result;
            }
            
            #endregion
            
            return Pop();
        }
    }
}