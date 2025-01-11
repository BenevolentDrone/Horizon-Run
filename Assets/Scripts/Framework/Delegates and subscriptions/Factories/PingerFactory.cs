using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Collections;
using HereticalSolutions.Delegates;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class PingerFactory
    {
        private const int DEFAULT_PINGER_CAPACITY = 16;

        //TODO: make thread safe
        private static ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder;

        #region Pinger
        
        public static Pinger BuildPinger(
            IPool<PingerInvocationContext> contextPool)
        {
            return new Pinger(
                contextPool);
        }

        #endregion
        
        #region Non alloc pinger
        
        public static NonAllocPinger BuildNonAllocPinger(
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_PINGER_CAPACITY
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                
                null,
                null);

            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();
            
            return BuildNonAllocPinger(
                subscriptionsPool,
                loggerResolver);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                initial,
                additional,
                
                null,
                null);
            
            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();

            return BuildNonAllocPinger(
                subscriptionsPool,
                loggerResolver);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            IManagedPool<INonAllocSubscription> subscriptionsPool,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPinger>();

            IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents =
                subscriptionsPool as IDynamicArray<IPoolElementFacade<INonAllocSubscription>>;
            
            return new NonAllocPinger(
                subscriptionsPool,
                subscriptionsContents,
                logger);
        }
        
        #endregion
    }
}