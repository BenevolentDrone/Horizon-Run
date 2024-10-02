using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Collections;
using HereticalSolutions.Delegates.Pinging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class PingersFactory
    {
        private const int DEFAULT_PINGER_CAPACITY = 16;

        //TODO: make thread safe
        private static ManagedPoolBuilder<ISubscription> managedPoolBuilder;

        #region Pinger
        
        public static Pinger BuildPinger()
        {
            return new Pinger();
        }

        #endregion
        
        #region Non alloc pinger
        
        public static NonAllocPinger BuildNonAllocPinger(
            ILoggerResolver loggerResolver = null)
        {
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<ISubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<ISubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
            Func<ISubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<ISubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<ISubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<ISubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
            IManagedPool<ISubscription> subscriptionsPool,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPinger>()
                ?? null;

            IDynamicArray<IPoolElementFacade<ISubscription>> subscriptionsContents =
                subscriptionsPool as IDynamicArray<IPoolElementFacade<ISubscription>>;
            
            return new NonAllocPinger(
                subscriptionsPool,
                subscriptionsContents,
                logger);
        }
        
        #endregion
    }
}