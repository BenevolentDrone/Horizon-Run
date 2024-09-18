using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Pools
{
    public abstract class ADecoratorPool<T>
        : IPool<T>,
          ICleanuppable,
          IDisposable
    {
        protected readonly IPool<T> innerPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADecoratorPool{T}"/> class with the specified inner pool.
        /// </summary>
        /// <param name="innerPool">The inner pool to decorate.</param>
        public ADecoratorPool(IPool<T> innerPool)
        {
            this.innerPool = innerPool;
        }

        #region IPool

        public virtual T Pop()
        {
            OnBeforePop(null);

            T result = innerPool.Pop();

            OnAfterPop(result, null);

            return result;
        }

        public virtual T Pop(IPoolPopArgument[] args)
        {
            OnBeforePop(args);

            T result = innerPool.Pop(args);

            OnAfterPop(result, args);

            return result;
        }
        
        public virtual void Push(
            T instance)
        {
            OnBeforePush(instance);

            innerPool.Push(
                instance);

            OnAfterPush(instance);
        }
        
        #endregion

        /// <summary>
        /// Called before an object is popped from the pool.
        /// </summary>
        /// <param name="args">The arguments provided for object decoration.</param>
        protected virtual void OnBeforePop(IPoolPopArgument[] args)
        {
        }

        /// <summary>
        /// Called after an object is popped from the pool.
        /// </summary>
        /// <param name="instance">The popped object from the pool.</param>
        /// <param name="args">The arguments provided for object decoration.</param>
        protected virtual void OnAfterPop(
            T instance,
            IPoolPopArgument[] args)
        {
        }

        /// <summary>
        /// Called before an object is pushed back into the pool.
        /// </summary>
        /// <param name="instance">The object to push back into the pool.</param>
        protected virtual void OnBeforePush(T instance)
        {
        }

        /// <summary>
        /// Called after an object is pushed back into the pool.
        /// </summary>
        /// <param name="instance">The object pushed back into the pool.</param>
        protected virtual void OnAfterPush(T instance)
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