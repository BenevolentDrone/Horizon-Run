using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class NonAllocBroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          INonAllocSubscribableMultipleArgs,
          ICleanuppable,
          IDisposable
    {
        #region Subscriptions

        private readonly IManagedPool<ISubscription> subscriptionsPool;

        private readonly IDynamicArray<IPoolElementFacade<ISubscription>> subscriptionsContents;

        #endregion

        private readonly ILogger logger;

        #region Buffer

        private ISubscription[] currentSubscriptionsBuffer;

        private int currentSubscriptionsBufferCount = -1;

        #endregion

        private bool broadcastInProgress = false;

        public NonAllocBroadcasterMultipleArgs(
            IManagedPool<ISubscription> subscriptionsPool,
            IDynamicArray<IPoolElementFacade<ISubscription>> subscriptionsContents,
            ILogger logger = null)
        {
            this.subscriptionsPool = subscriptionsPool;

            this.logger = logger;

            this.subscriptionsContents = subscriptionsContents;

            currentSubscriptionsBuffer = new ISubscription[subscriptionsContents.Capacity];
        }

        #region INonAllocSubscribableMultipleArgs

        public void Subscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            var subscriptionState = (ISubscriptionState<IInvokableMultipleArgs>)subscription;

            subscriptionElement.Value = (ISubscription)subscriptionState;

            subscription.Activate(this, subscriptionElement);

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()}");
        }

        public void Unsubscribe(
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>
                subscription)
        {
            if (!subscription.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscription.Terminate();

            logger?.Log<NonAllocBroadcasterMultipleArgs>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()}");
        }

        IEnumerable<
            ISubscriptionHandler<
                INonAllocSubscribableMultipleArgs,
                IInvokableMultipleArgs>>
                INonAllocSubscribableMultipleArgs.AllSubscriptions
        {
            get
            {
                var allSubscriptions = new ISubscriptionHandler<
                    INonAllocSubscribableMultipleArgs,
                    IInvokableMultipleArgs>
                    [subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = (ISubscriptionHandler<
                        INonAllocSubscribableMultipleArgs,
                        IInvokableMultipleArgs>)
                        subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        #region INonAllocSubscribable

        IEnumerable<ISubscription> INonAllocSubscribable.AllSubscriptions
        {
            get
            {
                ISubscription[] allSubscriptions = new ISubscription[subscriptionsContents.Count];

                for (int i = 0; i < allSubscriptions.Length; i++)
                    allSubscriptions[i] = subscriptionsContents[i].Value;

                return allSubscriptions;
            }
        }

        public void UnsubscribeAll()
        {
            while (subscriptionsContents.Count > 0)
            {
                var subscription = (ISubscriptionHandler<
                    INonAllocSubscribableMultipleArgs,
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
            //should NOT be changed during the invokation
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

        private void TryRemoveFromBuffer(IPoolElementFacade<ISubscription> subscriptionElement)
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
                currentSubscriptionsBuffer = new ISubscription[subscriptionsContents.Capacity];
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
                    var subscriptionState = (ISubscriptionState<IInvokableMultipleArgs>)currentSubscriptionsBuffer[i];

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