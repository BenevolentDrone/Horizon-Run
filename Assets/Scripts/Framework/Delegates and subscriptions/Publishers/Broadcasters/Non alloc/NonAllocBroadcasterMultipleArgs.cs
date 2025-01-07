using System;
using System.Collections.Generic;

using HereticalSolutions.Pools;

using HereticalSolutions.Bags;

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
        private readonly IBag<INonAllocSubscription> subscriptionsBag;

        private readonly IPool<NonAllocBroadcasterMultipleArgsInvocationContext> contextPool;

        private readonly ILogger logger;

        public NonAllocBroadcasterMultipleArgs(
            IBag<INonAllocSubscription> subscriptionsBag,
            IPool<NonAllocBroadcasterMultipleArgsInvocationContext> contextPool,
            ILogger logger = null)
        {
            this.subscriptionsBag = subscriptionsBag;

            this.contextPool = contextPool;

            this.logger = logger;
        }

        #region INonAllocSubscribable

        public bool Subscribe(
            INonAllocSubscription subscription)
        {
            var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

            if (subscriptionContext == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                return false;
            }

            if (!subscriptionContext.ValidateActivation(this))
                return false;

            if (!subscriptionsBag.Push(
                subscription))
                return false;

            subscriptionContext.Activate(
                this);

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION {subscription.GetHashCode()} ADDED: {this.GetHashCode()}");

            return true;
        }

        public bool Unsubscribe(
            INonAllocSubscription subscription)
        {
            var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

            if (subscriptionContext == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

                return false;
            }

            if (!subscriptionContext.ValidateActivation(this))
                return false;

            if (!subscriptionsBag.Pop(
                subscription))
                return false;

            subscriptionContext.Terminate();

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION {subscription.GetHashCode()} REMOVED: {this.GetHashCode()}");

            return true;
        }

        public IEnumerable<INonAllocSubscription> AllSubscriptions
        {
            get => subscriptionsBag.All;
        }

        public void UnsubscribeAll()
        {
            foreach (var subscription in subscriptionsBag.All)
            {
                var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

                if (subscriptionContext == null)
                    continue;

                if (!subscriptionContext.ValidateActivation(this))
                    continue;

                subscriptionContext.Terminate();
            }

            subscriptionsBag.Clear();
        }

        #endregion

        #region IPublisherMultipleArgs

        public void Publish(
            object[] values)
        {
            if (subscriptionsBag.Count == 0)
                return;

            //Pop context out of the pool and initialize it with values from the bag

            var count = subscriptionsBag.Count;

            var context = contextPool.Pop();

            bool newBuffer = false;

            if (context.Subscriptions == null)
            {
                context.Subscriptions = new INonAllocSubscription[count];

                newBuffer = true;
            }

            if (context.Subscriptions.Length < subscriptionsBag.Count)
            {
                context.Subscriptions = new INonAllocSubscription[subscriptionsBag.Count];

                newBuffer = true;
            }

            if (!newBuffer)
            {
                for (int i = 0; i < context.Count; i++)
                {
                    context.Subscriptions[i] = null;
                }
            }

            int index = 0;

            //TODO: remove foreach
            foreach (var subscription in subscriptionsBag.All)
            {
                context.Subscriptions[index] = subscription;

                index++;
            }

            context.Count = count;


            //Invoke the delegates in the context

            for (int i = 0; i < context.Count; i++)
            {
                var subscriptionContext = context.Subscriptions[i] as INonAllocSubscriptionContext<IInvokableMultipleArgs>;

                if (subscriptionContext == null)
                    continue;

                subscriptionContext.Delegate?.Invoke(values);
            }


            //Clean up and push the context back into the pool

            for (int i = 0; i < count; i++)
            {
                context.Subscriptions[i] = null;
            }

            context.Count = 0;

            contextPool.Push(context);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (subscriptionsBag is ICleanuppable)
                (subscriptionsBag as ICleanuppable).Cleanup();

            if (contextPool is ICleanuppable)
                (contextPool as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (subscriptionsBag is IDisposable)
                (subscriptionsBag as IDisposable).Dispose();

            if (contextPool is IDisposable)
                (contextPool as IDisposable).Dispose();
        }

        #endregion
    }
}