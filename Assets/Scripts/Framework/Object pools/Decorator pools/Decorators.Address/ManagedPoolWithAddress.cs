using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
    public class ManagedPoolWithAddress<T>
        : IManagedPool<T>,
          ICleanuppable,
          IDisposable
    {
        private readonly int level;

        private readonly IRepository<int, IManagedPool<T>> innerPoolRepository;

        private readonly PoolWithAddressBuilder<T> builder;

        private readonly ILogger logger;

        public ManagedPoolWithAddress(
            IRepository<int, IManagedPool<T>> innerPoolRepository,
            int level,
            ILogger logger,
            PoolWithAddressBuilder<T> builder = null)
        {
            this.innerPoolRepository = innerPoolRepository;

            this.level = level;
            
            this.builder = builder;
            
            this.logger = logger;
            
            if (this.builder != null)
                this.builder.Initialize(this);
        }

        #region IManagedPool

        public IPoolElementFacade<T> Pop()
        {
            throw new Exception(
                logger.TryFormatException(
                    GetType(),
                    "ADDRESS ARGUMENT ABSENT"));
        }

        public IPoolElementFacade<T> Pop(IPoolPopArgument[] args)
        {
            #region Validation

            if (args == null
                || !args.TryGetArgument<AddressArgument>(out var arg))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "ADDRESS ARGUMENT ABSENT"));

            if (arg.AddressHashes.Length < level)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID ADDRESS DEPTH. LEVEL: {{ {level} }} ADDRESS LENGTH: {{ {arg.AddressHashes.Length} }}"));

            #endregion

            IManagedPool<T> poolByAddress = null;

            #region Pool at the end of address

            if (arg.AddressHashes.Length == level)
            {
                if (!innerPoolRepository.TryGet(
                        0,
                        out poolByAddress))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"NO POOL DETECTED AT THE END OF ADDRESS. LEVEL: {{ {level} }}"));

                var endOfAddressResult = poolByAddress.Pop(args);

                return endOfAddressResult;
            }

            #endregion

            #region Pool at current level of address

            int currentAddressHash = arg.AddressHashes[level];

            if (!innerPoolRepository.TryGet(
                    currentAddressHash,
                    out poolByAddress))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID ADDRESS {{ {arg.FullAddress} }} ADDRESS HASH: {{ {currentAddressHash} }} LEVEL: {{ {level} }}"));

            var result = poolByAddress.Pop(args);

            return result;

            #endregion
        }

        public void Push(
            IPoolElementFacade<T> instance)
        {
            IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
                instance as IPoolElementFacadeWithMetadata<T>;

            if (instanceWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
			
            if (!instanceWithMetadata.Metadata.Has<IContainsAddress>())
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO ADDRESS METADATA"));
            
            IManagedPool<T> pool = null;

            var addressHashes = instanceWithMetadata.Metadata.Get<IContainsAddress>().AddressHashes;

            if (addressHashes.Length == level)
            {
                if (!innerPoolRepository.TryGet(
                        0,
                        out pool))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"NO POOL DETECTED AT ADDRESS MAX. DEPTH. LEVEL: {{ {level} }}"));

                pool.Push(instance);
                
                return;
            }

            int currentAddressHash = addressHashes[level];

            if (!innerPoolRepository.TryGet(currentAddressHash, out pool))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID ADDRESS {{ {currentAddressHash} }}"));

            pool.Push(instance);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPoolRepository is ICleanuppable)
                (innerPoolRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPoolRepository is IDisposable)
                (innerPoolRepository as IDisposable).Dispose();
        }

        #endregion

        public IRepository<int, IManagedPool<T>> InnerPoolRepository { get => innerPoolRepository; }

        public void AddPool(
            string address,
            IManagedPool<T> pool)
        {
            builder?.Parse(
                address,
                pool);
            
            builder?.Validate();
        }
    }
}