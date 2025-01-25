using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

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

        public const int DEFAULT_INVOKATION_CONTEXT_SIZE = 32;

        public static int InvokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;

        #endregion

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

        #region Broadcaster generic

        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver)
        {
            var contextPool = BuildContextPool<BroadcasterInvocationContext<T>>(
                loggerResolver);

            return BuildBroadcasterGeneric<T>(
                contextPool,
                loggerResolver);
        }

        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            IPool<BroadcasterInvocationContext<T>> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterGeneric<T>>();

            return new BroadcasterGeneric<T>(
                contextPool,
                logger);
        }

        #endregion

        #region Concurrent broadcaster generic

        public static ConcurrentBroadcasterGeneric<T> BuildConcurrentBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver)
        {
            var contextPool = BuildContextPool<BroadcasterInvocationContext<T>>(
                loggerResolver);

            return BuildConcurrentBroadcasterGeneric<T>(
                contextPool,
                loggerResolver);
        }

        public static ConcurrentBroadcasterGeneric<T> BuildConcurrentBroadcasterGeneric<T>(
            IPool<BroadcasterInvocationContext<T>> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentBroadcasterGeneric<T>>();

            return new ConcurrentBroadcasterGeneric<T>(
                contextPool,
                logger);
        }

        #endregion

        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    loggerResolver));
        }

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            IPool<BroadcasterInvocationContext<object[]>> contextPool,
            ILoggerResolver loggerResolver)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    contextPool,
                    loggerResolver));
        }

        #endregion

        #region Concurrent broadcaster multiple args

        public static ConcurrentBroadcasterMultipleArgs BuildConcurrentBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver)
        {
            return new ConcurrentBroadcasterMultipleArgs(
                BuildConcurrentBroadcasterGeneric<object[]>(
                    loggerResolver));
        }

        public static ConcurrentBroadcasterMultipleArgs BuildConcurrentBroadcasterMultipleArgs(
            IPool<BroadcasterInvocationContext<object[]>> contextPool,
            ILoggerResolver loggerResolver)
        {
            return new ConcurrentBroadcasterMultipleArgs(
                BuildConcurrentBroadcasterGeneric<object[]>(
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

        #region Concurrent broadcaster with repository

        public static ConcurrentBroadcasterWithRepository BuildConcurrentBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository,
            ILoggerResolver loggerResolver)
        {
            return BuildConcurrentBroadcasterWithRepository(
                RepositoryFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }

        public static ConcurrentBroadcasterWithRepository BuildConcurrentBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentBroadcasterWithRepository>();

            return new ConcurrentBroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion

        #region Non alloc broadcaster generic

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocBroadcasterGeneric<T>(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildNonAllocBroadcasterGeneric<T>(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterGeneric<T>>();

            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsBag,
                contextPool,
                logger);
        }

        #endregion

        #region Concurrent non alloc broadcaster generic

        public static ConcurrentNonAllocBroadcasterGeneric<T> BuildConcurrentNonAllocBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver)
        {
            return BuildConcurrentNonAllocBroadcasterGeneric<T>(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static ConcurrentNonAllocBroadcasterGeneric<T> BuildConcurrentNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildConcurrentNonAllocBroadcasterGeneric<T>(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static ConcurrentNonAllocBroadcasterGeneric<T> BuildConcurrentNonAllocBroadcasterGeneric<T>(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterGeneric<T>>();

            return new ConcurrentNonAllocBroadcasterGeneric<T>(
                subscriptionsBag,
                contextPool,
                logger);
        }

        #endregion

        #region Async broadcaster generic

        public static AsyncBroadcasterGeneric<T> BuildAsyncBroadcasterGeneric<T>(
            ILoggerResolver loggerResolver)
        {
            return BuildAsyncBroadcasterGeneric<T>(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static AsyncBroadcasterGeneric<T> BuildAsyncBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildAsyncBroadcasterGeneric<T>(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static AsyncBroadcasterGeneric<T> BuildAsyncBroadcasterGeneric<T>(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AsyncBroadcasterGeneric<T>>();

            return new AsyncBroadcasterGeneric<T>(
                subscriptionsBag,
                contextPool,
                logger);
        }

        #endregion

        #region Non alloc broadcaster multiple args

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocBroadcasterMultipleArgs(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildNonAllocBroadcasterMultipleArgs(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>();

            return new NonAllocBroadcasterMultipleArgs(
                subscriptionsBag,
                contextPool,
                logger);
        }

        #endregion

        #region Concurrent non alloc broadcaster multiple args

        public static ConcurrentNonAllocBroadcasterMultipleArgs BuildConcurrentNonAllocBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver)
        {
            return BuildConcurrentNonAllocBroadcasterMultipleArgs(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static ConcurrentNonAllocBroadcasterMultipleArgs BuildConcurrentNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildConcurrentNonAllocBroadcasterMultipleArgs(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static ConcurrentNonAllocBroadcasterMultipleArgs BuildConcurrentNonAllocBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterMultipleArgs>();

            return new ConcurrentNonAllocBroadcasterMultipleArgs(
                subscriptionsBag,
                contextPool,
                logger);
        }

        #endregion

        #region Async broadcaster multiple args

        public static AsyncBroadcasterMultipleArgs BuildAsyncBroadcasterMultipleArgs(
            ILoggerResolver loggerResolver)
        {
            return BuildAsyncBroadcasterMultipleArgs(
                BroadcasterSubscriptionPoolInitialAllocationDescriptor,
                BroadcasterSubscriptionPoolAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static AsyncBroadcasterMultipleArgs BuildAsyncBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocBroadcasterInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocBroadcasterInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildAsyncBroadcasterMultipleArgs(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocBroadcasterInvocationContext>(
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocBroadcasterInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static AsyncBroadcasterMultipleArgs BuildAsyncBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>();

            return new AsyncBroadcasterMultipleArgs(
                subscriptionsBag,
                contextPool,
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

        #region Concurrent non alloc broadcaster with repository

        public static ConcurrentNonAllocBroadcasterWithRepository BuildConcurrentNonAllocBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository,
            ILoggerResolver loggerResolver)
        {
            return BuildConcurrentNonAllocBroadcasterWithRepository(
                RepositoryFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }

        public static ConcurrentNonAllocBroadcasterWithRepository BuildConcurrentNonAllocBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterWithRepository>();

            return new ConcurrentNonAllocBroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion

        #region Async broadcaster with repository

        public static AsyncBroadcasterWithRepository BuildAsyncBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository,
            ILoggerResolver loggerResolver)
        {
            return BuildAsyncBroadcasterWithRepository(
                RepositoryFactory.BuildDictionaryInstanceRepository(
                    broadcasterRepository),
                loggerResolver);
        }

        public static AsyncBroadcasterWithRepository BuildAsyncBroadcasterWithRepository(
            IReadOnlyInstanceRepository repository,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AsyncBroadcasterWithRepository>();

            return new AsyncBroadcasterWithRepository(
                repository,
                logger);
        }

        #endregion
    }
}