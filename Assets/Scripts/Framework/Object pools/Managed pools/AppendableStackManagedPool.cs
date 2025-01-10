using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class AppendableStackManagedPool<T>
        : StackManagedPool<T>
    {
        private readonly AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand;
        
        private readonly AllocationCommand<T> nullValueAllocationCommand;
        
        public AppendableStackManagedPool(
            Stack<IPoolElementFacade<T>> pool,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            
            AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand,
            AllocationCommand<T> nullValueAllocationCommand,
                
            ILogger logger = null)
            : base (
                pool,
                facadeAllocationCommand,
                valueAllocationCommand,
                logger)
        {
            this.appendFacadeAllocationCommand = appendFacadeAllocationCommand;
            
            this.nullValueAllocationCommand = nullValueAllocationCommand;
        }
        
        public override IPoolElementFacade<T> Pop(
            IPoolPopArgument[] args)
        {
            #region Append from argument
            
            if (args.TryGetArgument<AppendToPoolArgument>(out var arg))
            {
                logger?.Log(
                    GetType(),
                    "APPEND ARGUMENT RECEIVED, APPENDING");

                capacity = StackPoolFactory.ResizeStackManagedPool(
                    pool,
                    capacity,
                    appendFacadeAllocationCommand,
                    nullValueAllocationCommand,
                    logger);
                        
                var result = pool.Pop();
                
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