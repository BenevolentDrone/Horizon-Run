using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Collections;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class AppendableLinkedListManagedPool<T> :
        LinkedListManagedPool<T>
    {
        private readonly AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand;
        
        private readonly AllocationCommand<T> nullValueAllocationCommand;
        
        public AppendableLinkedListManagedPool(
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            
            AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand,
            AllocationCommand<T> nullValueAllocationCommand,
                
            ILinkedListLink<T> firstElement,
            int capacity,
            ILogger logger = null)
            : base (
                facadeAllocationCommand,
                valueAllocationCommand,
                firstElement,
                capacity,
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
                logger?.Log<AppendableLinkedListManagedPool<T>>(
                    "APPEND ARGUMENT RECEIVED, APPENDING");

                LinkedListPoolFactory.ResizeLinkedListManagedPool(
                    ref firstElement,
                    ref capacity,
                    appendFacadeAllocationCommand,
                    nullValueAllocationCommand,
                    logger);
                

                var poppedElement = firstElement;
            
                firstElement = poppedElement.Next;
            
                poppedElement.Previous = null;
            
                poppedElement.Next = null;
            
                if (firstElement != null)
                    firstElement.Previous = null;
            
            
                IPoolElementFacade<T> result = poppedElement as IPoolElementFacade<T>;

                if (result == null)
                {
                    throw new Exception(
                        logger.TryFormatException<AppendableLinkedListManagedPool<T>>(
                            "LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
                }
                
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