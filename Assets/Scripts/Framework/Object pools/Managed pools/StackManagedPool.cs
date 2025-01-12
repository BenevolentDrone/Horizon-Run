using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class StackManagedPool<T> 
        : IManagedPool<T>,
          IAllocationResizeable,
          ICleanuppable,
          IDisposable
    {
        protected readonly Stack<IPoolElementFacade<T>> pool;

        private readonly AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand;
        
        private readonly AllocationCommand<T> valueAllocationCommand;
        
        protected readonly ILogger logger;

        protected int capacity;
        
        public StackManagedPool(
            Stack<IPoolElementFacade<T>> pool,
            AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            AllocationCommand<T> valueAllocationCommand,
            ILogger logger = null)
        {
            this.pool = pool;

            this.facadeAllocationCommand = facadeAllocationCommand;

            this.valueAllocationCommand = valueAllocationCommand;

            this.logger = logger;

            capacity = this.pool.Count;
        }

        #region IManagedPool

        public IPoolElementFacade<T> Pop()
        {
            IPoolElementFacade<T> result = null;

            if (pool.Count != 0)
            {
                result = pool.Pop();
            }
            else
            {
                capacity = StackPoolFactory.ResizeStackManagedPool(
                    pool,
                    capacity,
                    facadeAllocationCommand,
                    valueAllocationCommand,
                    logger);

                result = pool.Pop();
            }
            
            //Validate values

            if (result.Status == EPoolElementStatus.UNINITIALIZED)
            {
                var newElement = valueAllocationCommand.AllocationDelegate(); 
                
                valueAllocationCommand.AllocationCallback?.OnAllocated(newElement);
                
                result.Value = newElement;
            }
            
            //Validate pool

            if (result.Pool == null)
            {
                result.Pool = this;
            }

            //Update facade

            result.Status = EPoolElementStatus.POPPED;
            
            return result;
        }
        
        public virtual IPoolElementFacade<T> Pop(IPoolPopArgument[] args)
        {
            return Pop();
        }

        public void Push(IPoolElementFacade<T> instance)
        {
            // Validate values

            if (instance.Status != EPoolElementStatus.POPPED)
            {
                return;
            }
            
            //Update facade
            
            instance.Status = EPoolElementStatus.PUSHED;
            
            pool.Push(instance);
        }

        #endregion

        #region IAllocationResizeable

        public void Resize()
        {
            capacity = StackPoolFactory.ResizeStackManagedPool(
                pool,
                capacity,
                facadeAllocationCommand,
                valueAllocationCommand,
                logger);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var item in pool)
                if (item is ICleanuppable)
                    (item as ICleanuppable).Cleanup();

            pool.Clear();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var item in pool)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            pool.Clear();
        }

        #endregion
    }
}