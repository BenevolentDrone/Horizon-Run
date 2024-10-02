using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public abstract class ADecoratorManagedPool<T>
        : IManagedPool<T>,
          ICleanuppable,
          IDisposable
    {
        protected readonly IManagedPool<T> innerPool;
        
        protected readonly ILogger logger;

        public ADecoratorManagedPool(
            IManagedPool<T> innerPool,
            ILogger logger = null)
        {
            this.innerPool = innerPool;

            this.logger = logger;
        }

        #region IManagedPool

        public virtual IPoolElementFacade<T> Pop()
        {
            OnBeforePop(null);

            IPoolElementFacade<T> result = innerPool.Pop();

            OnAfterPop(
                result,
                null);

            return result;
        }

        public virtual IPoolElementFacade<T> Pop(IPoolPopArgument[] args)
        {
            OnBeforePop(args);

            IPoolElementFacade<T> result = innerPool.Pop(args);

            OnAfterPop(result, args);

            return result;
        }
        
        public virtual void Push(
            IPoolElementFacade<T> instance)
        {
            OnBeforePush(instance);

            innerPool.Push(
                instance);

            OnAfterPush(instance);
        }
        
        #endregion

        /// <summary>
        /// Invoked before an object is retrieved from the pool.
        /// </summary>
        /// <param name="args">The arguments for applying decorators.</param>
        protected virtual void OnBeforePop(IPoolPopArgument[] args)
        {
        }

        /// <summary>
        /// Invoked after an object is retrieved from the pool.
        /// </summary>
        /// <param name="instance">The retrieved object.</param>
        /// <param name="args">The arguments for applying decorators.</param>
        protected virtual void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
        }

        /// <summary>
        /// Invoked before an object is returned to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        protected virtual void OnBeforePush(IPoolElementFacade<T> instance)
        {
        }

        /// <summary>
        /// Invoked after an object is returned to the pool.
        /// </summary>
        /// <param name="instance">The object to be returned to the pool.</param>
        protected virtual void OnAfterPush(IPoolElementFacade<T> instance)
        {
        }

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPool is ICleanuppable)
                (innerPool as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPool is IDisposable)
                (innerPool as IDisposable).Dispose();
        }

        #endregion
    }
}