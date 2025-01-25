using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class BroadcasterFactory
    {
        #region Factory settings

        #region Broadcaster context pool

        public const int DEFAULT_BROADCASTER_CONTEXT_POOL_SIZE = 32;

        public static AllocationCommandDescriptor BroadcasterContextPoolInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_BROADCASTER_CONTEXT_POOL_SIZE
            };

        public static AllocationCommandDescriptor BroadcasterContextPoolAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_BROADCASTER_CONTEXT_POOL_SIZE
            };

        #endregion

        #region Broadcaster subscriptions pool

        private const int DEFAULT_BROADCASTER_SUBSCRIPTION_POOL_SIZE = 32;

        public static AllocationCommandDescriptor BroadcasterSubscriptionPoolInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_BROADCASTER_SUBSCRIPTION_POOL_SIZE
            };

        public static AllocationCommandDescriptor BroadcasterSubscriptionPoolAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_BROADCASTER_SUBSCRIPTION_POOL_SIZE
            };

        #endregion

        #endregion

        public static ManagedPoolBuilder<T> BuildManagedPoolBuilder<T>(
            ILoggerResolver loggerResolver)
        {
            return new ManagedPoolBuilder<T>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<T>>());
        }

        public static IPool<T> BuildContextPool<T>(
            ILoggerResolver loggerResolver)
        {
            Func<T> valueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<T>;

            return StackPoolFactory.BuildStackPool<T>(
                new AllocationCommand<T>
                {
                    Descriptor = BroadcasterContextPoolInitialAllocationDescriptor,

                    AllocationDelegate = valueAllocationDelegate
                },
                new AllocationCommand<T>
                {
                    Descriptor = BroadcasterContextPoolAdditionalAllocationDescriptor,
                    
                    AllocationDelegate = valueAllocationDelegate
                },
                loggerResolver);
        }

        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver,

            IPool<BroadcasterGenericInvocationContext<object[]>> contextPool = null)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    contextPool,
                    loggerResolver));
        }

        #endregion
        
        #region Broadcaster with repository

        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository,
            ILoggerResolver loggerResolver)
        {
            return BuildBroadcasterWithRepository(
                RepositoryFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }
        
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterWithRepository>();

            return new BroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion
        
        #region Broadcaster generic

        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver,
            
            IPool<BroadcasterGenericInvocationContext<T>> contextPool = null)
        {
            if (contextPool == null)
                contextPool = BuildContextPool<BroadcasterGenericInvocationContext<T>>(
                    loggerResolver);

            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterGeneric<T>>();

            return new BroadcasterGeneric<T>(
                contextPool,
                logger);
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder,
            ILoggerResolver loggerResolver)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver);

            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_BROADCASTER_SUBSCRIPTION_POOL_SIZE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                
                null,
                null);

            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();
            
            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                loggerResolver);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver,

            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder<INonAllocSubscription>(
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
            
            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();

            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                loggerResolver);
        }
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionsPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>();
            
            IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents =
                subscriptionsPool as IDynamicArray<IPoolElementFacade<INonAllocSubscription>>;

            return new NonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                subscriptionsContents,
                logger);
        }
        
        #endregion
        
        #region Non alloc broadcaster with repository
        
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository,
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocBroadcasterWithRepository(
                RepositoryFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }
        
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterWithRepository>();

            return new NonAllocBroadcasterWithRepository(
                repository,
                logger);
        }
        
        #endregion
        
        #region Non alloc broadcaster generic
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver,

            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver);

            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                
                null,
                null);

            var subscriptionPool = managedPoolBuilder.BuildPackedArrayManagedPool();
            
            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionPool,
                loggerResolver);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver,

            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate =
                AllocationFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = BuildManagedPoolBuilder<INonAllocSubscription>(
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
            
            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                loggerResolver);
        }
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver,

            IBag<INonAllocSubscription> subscriptionsBag = null,
            IPool<NonAllocBroadcasterGenericInvocationContext> contextPool = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterGeneric<T>>();

            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsBag,
                contextPool,
                logger);
        }
        
        #endregion
    }
}