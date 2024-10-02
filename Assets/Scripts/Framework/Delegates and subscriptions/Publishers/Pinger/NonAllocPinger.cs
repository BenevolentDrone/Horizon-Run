using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Pinging
{
    public class NonAllocPinger
        : IPublisherNoArgs,
          INonAllocSubscribableNoArgs,
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

        private bool pingInProgress = false;

        public NonAllocPinger(
            IManagedPool<ISubscription> subscriptionsPool,
            IDynamicArray<IPoolElementFacade<ISubscription>> subscriptionsContents,
            ILogger logger = null)
        {
            this.subscriptionsPool = subscriptionsPool;

            this.logger = logger;

            this.subscriptionsContents = subscriptionsContents;

            currentSubscriptionsBuffer = new ISubscription[subscriptionsContents.Capacity];
        }

        #region INonAllocSubscribableNoArgs

        public void Subscribe(ISubscription subscription)
        {
            var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

            if (!subscriptionHandler.ValidateActivation(this))
                return;

            var subscriptionElement = subscriptionsPool.Pop();

            subscriptionElement.Value = subscription;

            subscriptionHandler.Activate(this, subscriptionElement);

            logger?.Log<NonAllocPinger>(
                $"SUBSCRIPTION ADDED: {subscriptionElement.Value.GetHashCode()}");
        }

        public void Unsubscribe(ISubscription subscription)
        {
            var subscriptionHandler = (ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>)subscription;

            if (!subscriptionHandler.ValidateTermination(this))
                return;

            var poolElement = ((ISubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

            TryRemoveFromBuffer(poolElement);

            var previousValue = poolElement.Value;

            poolElement.Value = null;

            subscriptionsPool.Push(poolElement);

            subscriptionHandler.Terminate();

            logger?.Log<NonAllocPinger>(
                $"SUBSCRIPTION REMOVED: {previousValue.GetHashCode()}");
        }

        public void Unsubscribe(IPoolElementFacade<ISubscription> subscription)
        {
            TryRemoveFromBuffer(subscription);

            subscription.Value = null;

            subscriptionsPool.Push(subscription);
        }

        #region INonAllocSubscribable

        public IEnumerable<ISubscription> AllSubscriptions
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
                Unsubscribe(subscriptionsContents[0]);
        }

        #endregion

        #endregion

        private void TryRemoveFromBuffer(IPoolElementFacade<ISubscription> subscriptionElement)
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
            //should NOT be changed during the invokation
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
                currentSubscriptionsBuffer = new ISubscription[subscriptionsContents.Capacity];
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
                    var subscriptionState = (ISubscriptionState<IInvokableNoArgs>)currentSubscriptionsBuffer[i];

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