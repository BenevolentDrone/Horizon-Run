using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
    public class NonAllocPinger
        : IPublisherNoArgs,
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

        private bool pingInProgress = false;

        public NonAllocPinger(
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

        public void Subscribe(INonAllocSubscription subscription)
        {
            var subscriptionHandler = (INonAllocSubscriptionContext<>)subscription;

            if (!subscriptionHandler.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            subscriptionElement.Value = subscription;

            subscriptionHandler.Activate(this, subscriptionElement);

            logger?.Log<NonAllocPinger>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()}");
        }

        public void Unsubscribe(INonAllocSubscription subscription)
        {
            var subscriptionHandler = (INonAllocSubscriptionContext<>)subscription;

            if (!subscriptionHandler.ValidateTermination(this))
                return;

            var poolElement = ((INonAllocSubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscriptionHandler.Terminate();

            logger?.Log<NonAllocPinger>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()}");
        }

        public void Unsubscribe(IPoolElementFacade<INonAllocSubscription> subscription)
        {
            TryRemoveFromBuffer(subscription);

            subscription.Value = null;

            subscriptionsPool.Push(subscription);
        }

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
                Unsubscribe(subscriptionsContents[0]);
        }

        #endregion

        #endregion

        private void TryRemoveFromBuffer(IPoolElementFacade<INonAllocSubscription> subscriptionElement)
        {
            if (!pingInProgress)
                return;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                if (currentSubscriptionsBuffer[i] == subscriptionElement.Value)
                {
                    currentSubscriptionsBuffer[i] = null;

                    return;
                }
        }

        #region IPublisherNoArgs

        public void Publish()
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invocation
            //That's why we'll copy the subscriptions array to buffer and invoke it from there

            ValidateBufferSize();

            currentSubscriptionsBufferCount = subscriptionsContents.Count;

            CopySubscriptionsToBuffer();

            InvokeSubscriptions();

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

            pingInProgress = false;
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

        private void InvokeSubscriptions()
        {
            pingInProgress = true;

            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
            {
                if (currentSubscriptionsBuffer[i] != null)
                {
                    var subscriptionState = (INonAllocSubscriptionState<IInvokableNoArgs>)currentSubscriptionsBuffer[i];

                    subscriptionState.Invokable.Invoke();
                }
            }

            pingInProgress = false;
        }

        private void EmptyBuffer()
        {
            for (int i = 0; i < currentSubscriptionsBufferCount; i++)
                currentSubscriptionsBuffer[i] = null;
        }
    }
}