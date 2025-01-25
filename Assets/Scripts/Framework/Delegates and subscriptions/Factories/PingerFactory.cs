using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Bags;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class PingerFactory
    {
        #region Factory settings

        #region Pinger subscription pool

        public const int DEFAULT_PINGER_SUBSCRIPTION_POOL_CAPACITY = 32;

        public static AllocationCommandDescriptor PingerInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_PINGER_SUBSCRIPTION_POOL_CAPACITY
            };

        public static AllocationCommandDescriptor PingerAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_PINGER_SUBSCRIPTION_POOL_CAPACITY
            };

        #endregion

        #endregion

        #region Pinger
        
        public static Pinger BuildPinger(
            IPool<PingerInvocationContext> contextPool)
        {
            return new Pinger(
                contextPool);
        }

        #endregion
        
        #region Non alloc pinger
        
        public static ManagedPoolBuilder<INonAllocSubscription> BuildManagedPoolBuilder(
            ILoggerResolver loggerResolver)
        {
            return new ManagedPoolBuilder<INonAllocSubscription>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
        }

        public static NonAllocPinger BuildNonAllocPinger(
            ILoggerResolver loggerResolver,

            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder(
                    loggerResolver);
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                PingerInitialAllocationDescriptor,
                PingerAdditionalAllocationDescriptor,
                
                null,
                null);

            var subscriptionPool = managedPoolBuilder.BuildPackedArrayManagedPool();
            
            return BuildNonAllocPinger(
                subscriptionPool,
                loggerResolver);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver,
            
            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder(
                    loggerResolver);
            
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
            
            var subscriptionPool = managedPoolBuilder.BuildPackedArrayManagedPool();

            return BuildNonAllocPinger(
                subscriptionPool,
                loggerResolver);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            IBag<INonAllocSubscription> subscriptionBag,
            IPool<NonAllocPingerInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPinger>();

            return new NonAllocPinger(
                subscriptionBag,
                contextPool,
                logger);
        }
        
        #endregion
    }
}