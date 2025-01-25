using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Bags;

using HereticalSolutions.Logging;
using HereticalSolutions.Bags.Factories;

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

        public const int DEFAULT_INVOKATION_CONTEXT_SIZE = 32;

        public static int InvokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;

        #endregion

        #region Pinger

        public static Pinger BuildPinger(
            IPool<PingerInvocationContext> contextPool)
        {
            return new Pinger(
                contextPool);
        }

        #endregion

        #region Concurrent pinger

        public static ConcurrentPinger BuildConcurrentPinger(
            IPool<PingerInvocationContext> contextPool)
        {
            return new ConcurrentPinger(
                contextPool);
        }

        #endregion
        
        #region Non alloc pinger
        
        public static NonAllocPinger BuildNonAllocPinger(
            ILoggerResolver loggerResolver)
        {
            return BuildNonAllocPinger(
                PingerInitialAllocationDescriptor,
                PingerAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static NonAllocPinger BuildNonAllocPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocPingerInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocPingerInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildNonAllocPinger(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocPingerInvocationContext>(
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
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

        #region Concurrent non alloc pinger

        public static ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger(
            ILoggerResolver loggerResolver)
        {
            return BuildConcurrentNonAllocPinger(
                PingerInitialAllocationDescriptor,
                PingerAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocPingerInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocPingerInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildConcurrentNonAllocPinger(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocPingerInvocationContext>(
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger(
            IBag<INonAllocSubscription> subscriptionBag,
            IPool<NonAllocPingerInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentNonAllocPinger>();

            return new ConcurrentNonAllocPinger(
                subscriptionBag,
                contextPool,
                logger);
        }

        #endregion

        #region Async pinger

        public static AsyncPinger BuildAsyncPinger(
            ILoggerResolver loggerResolver)
        {
            return BuildAsyncPinger(
                PingerInitialAllocationDescriptor,
                PingerAdditionalAllocationDescriptor,
                loggerResolver);
        }

        public static AsyncPinger BuildAsyncPinger(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional,
            ILoggerResolver loggerResolver)
        {
            Func<NonAllocPingerInvocationContext> invocationContextAllocationDelegate =
                () => new NonAllocPingerInvocationContext
                {
                    Subscriptions = new INonAllocSubscription[InvokationContextSize]
                };

            return BuildAsyncPinger(
                LinkedListBagFactory.BuildNonAllocLinkedListBag<INonAllocSubscription>(
                    loggerResolver),
                StackPoolFactory.BuildStackPool<NonAllocPingerInvocationContext>(
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = initial,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    new AllocationCommand<NonAllocPingerInvocationContext>
                    {
                        Descriptor = additional,

                        AllocationDelegate = invocationContextAllocationDelegate
                    },
                    loggerResolver),
                loggerResolver);
        }

        public static AsyncPinger BuildAsyncPinger(
            IBag<INonAllocSubscription> subscriptionBag,
            IPool<NonAllocPingerInvocationContext> contextPool,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AsyncPinger>();

            return new AsyncPinger(
                subscriptionBag,
                contextPool,
                logger);
        }

        #endregion
    }
}