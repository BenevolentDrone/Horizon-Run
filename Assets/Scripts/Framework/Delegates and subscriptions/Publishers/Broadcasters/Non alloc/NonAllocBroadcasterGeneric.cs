using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
    public class NonAllocBroadcasterGeneric<TValue>
        : IPublisherSingleArgGeneric<TValue>,
          IPublisherSingleArg,
          INonAllocSubscribable,
          INonAllocSubscribableSingleArg,
          ICleanuppable,
          IDisposable
    {
        #region Subscriptions

        private readonly IManagedPool<INonAllocSubscription> subscriptionsPool;

        private readonly IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents;
        
        #endregion

        private readonly ILogger logger;

        #region Buffer

        private INonAllocSubscription[] currentSubscriptionsBuffer;

        private int currentSubscriptionsBufferCount = -1;

        #endregion

        private bool broadcastInProgress = false;

        public NonAllocBroadcasterGeneric(
            IManagedPool<INonAllocSubscription> subscriptionsPool,
            IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents,
            ILogger logger = null)
        {
            this.subscriptionsPool = subscriptionsPool;

            this.logger = logger;

            this.subscriptionsContents = subscriptionsContents;

            currentSubscriptionsBuffer = new INonAllocSubscription[subscriptionsContents.Capacity];
        }

        #region INonAllocSubscribableSingleArgGeneric

        public void Subscribe(
            INonAllocSubscription subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            var subscriptionState = (INonAllocSubscriptionState<IInvokableSingleArgGeneric<TValue>>)subscription;

            subscriptionElement.Value = (INonAllocSubscription)subscriptionState;

            subscription.Activate(this, subscriptionElement);

            logger?.Log<NonAllocBroadcasterGeneric<TValue>>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()} <{typeof(TValue).Name}>");
        }

        public void Unsubscribe(
            INonAllocSubscription subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((INonAllocSubscriptionState<IInvokableSingleArgGeneric<TValue>>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();

            logger?.Log<NonAllocBroadcasterGeneric<TValue>>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()} <{typeof(TValue).Name}>");
        }

        IEnumerable<INonAllocSubscription> INonAllocSubscribable.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>
                    [subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (INonAllocSubscriptionHandler<
                        INonAllocSubscribable,
                        IInvokableSingleArgGeneric<TValue>>)
                        subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        #endregion

        #region INonAllocSubscribableSingleArg

        public void Subscribe<TArgument>(
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableSingleArgGeneric<TArgument>>
                subscription)
        {
            switch (subscription)
            {
                case INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>> tValueSubscription:

                    Subscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Subscribe(
            Type valueType,
            INonAllocSubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            switch (subscription)
            {
                case INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Subscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        public void Unsubscribe<TArgument>(
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableSingleArgGeneric<TArgument>>
                subscription)
        {
            switch (subscription)
            {
                case INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Unsubscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Unsubscribe(
            Type valueType,
            INonAllocSubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>
                subscription)
        {
            switch (subscription)
            {
                case INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>
                    tValueSubscription:

                    Unsubscribe(tValueSubscription);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        IEnumerable<
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableSingleArgGeneric<TValue>>>
                INonAllocSubscribableSingleArg.GetAllSubscriptions<TValue>()
        {
            var allSubscriptions = new INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableSingleArgGeneric<TValue>>
                [subscriptionsContents.Count];

            for (int i = 0; i < allSubscriptions.Length; i++)
                allSubscriptions[i] = (INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>)
                    subscriptionsContents[i].Value;

            return allSubscriptions;
        }

        public IEnumerable<INonAllocSubscription> GetAllSubscriptions(Type valueType)
        {
            INonAllocSubscription[] allSubscriptions = new INonAllocSubscription[subscriptionsContents.Count];

            for (int i = 0; i < allSubscriptions.Length; i++)
                allSubscriptions[i] = subscriptionsContents[i].Value;

            return allSubscriptions;
        }

        IEnumerable<
            INonAllocSubscriptionHandler<
                INonAllocSubscribableSingleArg,
                IInvokableSingleArg>>
                INonAllocSubscribableSingleArg.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new INonAllocSubscriptionHandler<
                    INonAllocSubscribableSingleArg,
                    IInvokableSingleArg>
                    [subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (INonAllocSubscriptionHandler<
                        INonAllocSubscribableSingleArg,
                        IInvokableSingleArg>)
                        subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        #endregion

        #region INonAllocSubscribable

        public IEnumerable<INonAllocSubscription> AllSubscriptions
        {
            get
            {
                INonAllocSubscription[] allSubscriptions = new INonAllocSubscription[subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        public void UnsubscribeAll()
        {
            while (subscriptionsContents.Count > 0)
            {
                var subscription = (INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<TValue>>)
                    subscriptionsContents[0].Value;

                Unsubscribe(subscription);
            }
        }

        #endregion

        #region IPublisherSingleArgGeneric

        public void Publish(TValue value)
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invocation
            //That's why we'll copy the subscriptions array to buffer and invoke it from there

            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsContents.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions(value);

            EmptyBuffer();
        }

        #endregion

        #region IPublisherSingleArg

        public void Publish<TArgument>(TArgument value)
        {
            switch (value)
            {
                case TValue tValue:

                    Publish(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Publish(Type valueType, object value)
        {
            switch (value)
            {
                case TValue tValue:

                    Publish(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<NonAllocBroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (subscriptionsPool is ICleanuppable)
                (subscriptionsPool as ICleanuppable).Cleanup();

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null
                    && currentSubscriptionsBuffer[i] is ICleanuppable)
                {
                    (currentSubscriptionsBuffer[i] as ICleanuppable).Cleanup();
                }
            }
            
            EmptyBuffer();

            currentSubscriptionsBufferCount = -1;

            broadcastInProgress = false;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (subscriptionsPool is IDisposable)
                (subscriptionsPool as IDisposable).Dispose();

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null
                    && currentSubscriptionsBuffer[i] is IDisposable)
                {
                    (currentSubscriptionsBuffer[i] as IDisposable).Dispose();
                }
            }
        }

        #endregion

        private void TryRemoveFromBuffer(IPoolElementFacade<INonAllocSubscription> subscriptionElement)
        {
            if (!broadcastInProgress)
                return;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] == subscriptionElement.Value)
                {
                    currentSubscriptionsBuffer[i] = null;
                    return;
                }
            }
        }

        private void ValidateBufferSize()
        {
            if (currentSubscriptionsBuffer.Length < subscriptionsContents.Capacity)
                currentSubscriptionsBuffer = new INonAllocSubscription[subscriptionsContents.Capacity];
        }

        private void CopySubscriptionsToBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = subscriptionsContents[i].Value;
        }

        private void InvokeSubscriptions(TValue value)
        {
            broadcastInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (INonAllocSubscriptionContext<TValue>)currentSubscriptionsBuffer[i];

                    subscriptionState.Delegate.Invoke(value);
                }
            }

            broadcastInProgress = false;
        }

        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }
    }
}