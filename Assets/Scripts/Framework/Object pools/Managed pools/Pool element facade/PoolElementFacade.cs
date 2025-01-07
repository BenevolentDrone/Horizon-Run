using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Pools
{
    public class PoolElementFacade<T>
        : IPoolElementFacadeWithMetadata<T>,
          ICleanuppable,
          IDisposable
    {
        private readonly IStronglyTypedMetadata metadata;
        
        private EPoolElementStatus status;

        private IManagedPool<T> pool;
        
        public PoolElementFacade(
            IStronglyTypedMetadata metadata)
        {
            Value = default;

            status = EPoolElementStatus.UNINITIALIZED;

            this.metadata = metadata;
        }

        #region IPoolElementFacadeWithMetadata
        
        #region IPoolElementFacade

        public T Value { get; set; }

        public EPoolElementStatus Status
        {
            get => status;
            set => status = value;
        }

        public IManagedPool<T> Pool
        {
            get => pool;
            set => pool = value;
        }

        public void Push()
        {
            if (status == EPoolElementStatus.PUSHED)
                return;

            pool?.Push(this);
        }
        
        #endregion
        
        public IStronglyTypedMetadata Metadata
        {
            get => metadata;
        }
        
        #endregion

        #region ICleanUppable

        public virtual void Cleanup()
        {
            Push();

            if (Value is ICleanuppable)
                (Value as ICleanuppable).Cleanup();

            if (metadata is ICleanuppable)
                (metadata as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            Push();

            if (Value is IDisposable)
                (Value as IDisposable).Dispose();

            if (metadata is IDisposable)
                (metadata as IDisposable).Dispose();
        }

        #endregion
    }
}