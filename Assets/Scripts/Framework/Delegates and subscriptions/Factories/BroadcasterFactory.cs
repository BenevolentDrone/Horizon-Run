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
        private const int DEFAULT_BROADCASTER_CAPACITY = 16;
        
        public static ManagedPoolBuilder<T> BuildManagedPoolBuilder<T>(
            ILoggerResolver loggerResolver = null)
        {
            return new ManagedPoolBuilder<T>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<T>>());
        }

        public static IPool<T> BuildContextPool<T>(
            ILoggerResolver loggerResolver = null)
        {
            return StackPoolFactory.BuildStackPool<T>(
                new AllocationCommand<T>
                {
                    Descriptor = new AllocationCommandDescriptor
                    {
                        Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                        Amount = 32 //TODO: REMOVE MAGIC
                    },
                    AllocationDelegate =
                        AllocationFactory
                            .ActivatorAllocationDelegate<T>
                },
                new AllocationCommand<T>
                {
                    Descriptor = new AllocationCommandDescriptor
                    {
                        Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                        Amount = 32 //TODO: REMOVE MAGIC
                    },
                    AllocationDelegate =
                        AllocationFactory
                            .ActivatorAllocationDelegate<T>
                },
                loggerResolver);
        }

        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            IPool<BroadcasterGenericInvocationContext<object[]>> contextPool = null,
            ILoggerResolver loggerResolver = null)
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
            ILoggerResolver loggerResolver = null)
        {
            return BuildBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }
        
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver = null)
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
            IPool<BroadcasterGenericInvocationContext<T>> contextPool = null,
            ILoggerResolver loggerResolver = null)
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
            ILoggerResolver loggerResolver = null)
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

                    Amount = DEFAULT_BROADCASTER_CAPACITY
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
            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null,
            ILoggerResolver loggerResolver = null)
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
            ILoggerResolver loggerResolver = null)
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
            ILoggerResolver loggerResolver = null)
        {
            return BuildNonAllocBroadcasterWithRepository(
                RepositoriesFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }
        
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver = null)
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
            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null,
            ILoggerResolver loggerResolver = null)
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

                    Amount = DEFAULT_BROADCASTER_CAPACITY
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                
                null,
                null);

            var subscriptionsPool = managedPoolBuilder.BuildPackedArrayManagedPool();
            
            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                loggerResolver);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder = null,
            ILoggerResolver loggerResolver = null)
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
            IBag<INonAllocSubscription> subscriptionsBag = null,
            IPool<NonAllocBroadcasterGenericInvocationContext> contextPool = null,
            ILoggerResolver loggerResolver = null)
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