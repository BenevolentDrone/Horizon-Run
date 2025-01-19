using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class StackPool<T> 
        : IPool<T>,
          IAllocationResizeable,
          ICleanuppable,
          IDisposable
    {
        private readonly Stack<T> pool;

        private readonly AllocationCommand<T> allocationCommand;
        
        private readonly ILogger logger;

        private int capacity;
        
        public StackPool(
            Stack<T> pool,
            AllocationCommand<T> allocationCommand,
            ILogger logger)
        {
            this.pool = pool;

            this.allocationCommand = allocationCommand;

            this.logger = logger;

            capacity = this.pool.Count;
        }

        #region IPool

        public T Pop()
        {
            T result = default(T);

            if (pool.Count != 0)
            {
                result = pool.Pop();
            }
            else
            {
                capacity = StackPoolFactory.ResizeStackPool(
                    pool,
                    capacity,
                    allocationCommand,
                    logger);

                result = pool.Pop();
            }
            
            return result;
        }
        
        public T Pop(
            IPoolPopArgument[] args)
        {
            return Pop();
        }

        public void Push(
            T instance)
        {
            pool.Push(instance);
        }

        #endregion

        #region IAllocationResizeable

        public void Resize()
        {
            capacity = StackPoolFactory.ResizeStackPool(
                pool,
                capacity,
                allocationCommand,
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