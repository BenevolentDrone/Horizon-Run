using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Delegates;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class BroadcastersFactory
    {
        private const int DEFAULT_BROADCASTER_CAPACITY = 16;
        
        //TODO: make thread safe
        private static ManagedPoolBuilder<INonAllocSubscription> managedPoolBuilder;

        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver = null)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
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
                loggerResolver?.GetLogger<BroadcasterWithRepository>()
                ?? null;

            return new BroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion
        
        #region Broadcaster generic

        /// <summary>
        /// Builds a generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <returns>The built generic broadcaster.</returns>
        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterGeneric<T>>()
                ?? null;

            return new BroadcasterGeneric<T>(logger);
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
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

            return BuildNonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                loggerResolver);
        }
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            IManagedPool<INonAllocSubscription> subscriptionsPool,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>()
                ?? null;
            
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
        
        /// <summary>
        /// Builds a non-allocating broadcaster with a repository.
        /// </summary>
        /// <param name="repository">The repository for the broadcasters.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <returns>The built non-allocating broadcaster with a repository.</returns>
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterWithRepository>()
                ?? null;

            return new NonAllocBroadcasterWithRepository(
                repository,
                logger);
        }
        
        #endregion
        
        #region Non alloc broadcaster generic
        
        /// <summary>
        /// Builds a non-allocating generic broadcaster.
        /// </summary>
        /// <typeparam name="T">The type of the broadcast argument.</typeparam>
        /// <returns>The built non-allocating generic broadcaster.</returns>
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
            ILoggerResolver loggerResolver = null)
        {
            Func<INonAllocSubscription> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<INonAllocSubscription>;

            if (managedPoolBuilder == null)
                managedPoolBuilder = new ManagedPoolBuilder<INonAllocSubscription>(
                    loggerResolver,
                    loggerResolver?.GetLogger<ManagedPoolBuilder<INonAllocSubscription>>());
            
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

            return BuildNonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                loggerResolver);
        }
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            IManagedPool<INonAllocSubscription> subscriptionsPool,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterGeneric<T>>()
                ?? null;

            IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents =
                subscriptionsPool as IDynamicArray<IPoolElementFacade<INonAllocSubscription>>;
            
            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                subscriptionsContents,
                logger);
        }
        
        #endregion
    }
}