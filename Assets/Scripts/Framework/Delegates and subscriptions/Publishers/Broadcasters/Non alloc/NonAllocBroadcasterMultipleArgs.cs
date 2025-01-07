using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
    public class NonAllocBroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          INonAllocSubscribable,
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

        public NonAllocBroadcasterMultipleArgs(
            IManagedPool<INonAllocSubscription> subscriptionsPool,
            IDynamicArray<IPoolElementFacade<INonAllocSubscription>> subscriptionsContents,
            ILogger logger = null)
        {
            this.subscriptionsPool = subscriptionsPool;

            this.logger = logger;

            this.subscriptionsContents = subscriptionsContents;

            currentSubscriptionsBuffer = new INonAllocSubscription[subscriptionsContents.Capacity];
        }

        #region INonAllocSubscribable

        public void Subscribe(
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            var subscriptionState = (INonAllocSubscriptionState<IInvokableMultipleArgs>)subscription;

            subscriptionElement.Value = (INonAllocSubscription)subscriptionState;

            subscription.Activate(this, subscriptionElement);

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()}");
        }

        public void Unsubscribe(
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((INonAllocSubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()}");
        }

        IEnumerable<
            INonAllocSubscriptionHandler<
                INonAllocSubscribable,
                IInvokableMultipleArgs>>
                INonAllocSubscribable.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableMultipleArgs>
                    [subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (INonAllocSubscriptionHandler<
                        INonAllocSubscribable,
                        IInvokableMultipleArgs>)
                        subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        #region INonAllocSubscribable

        IEnumerable<INonAllocSubscription> INonAllocSubscribable.AllSubscriptions
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
                    IInvokableMultipleArgs>)
                    subscriptionsContents[0].Value;

                Unsubscribe(subscription);
            }
        }

        #endregion

        #endregion

        #region IPublisherMultipleArgs

        public void Publish(object[] value)
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

        private void InvokeSubscriptions(object[] value)
        {
            broadcastInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (INonAllocSubscriptionState<IInvokableMultipleArgs>)currentSubscriptionsBuffer[i];

                    subscriptionState.Invokable.Invoke(value);
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