using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionSingleArgGeneric<TValue>
        : ISubscription,
          ISubscriptionContext<IInvokableSingleArgGeneric<TValue>>,
          ISubscriptionContext<IInvokableSingleArg>,
          ICleanuppable,
          IDisposable
    {
        private readonly IInvokableSingleArgGeneric<TValue> invokable;

        private readonly ILogger logger;

        private object publisher;

        private IPoolElementFacade<ISubscription> poolElement;

        public SubscriptionSingleArgGeneric(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric(
                @delegate,
                loggerResolver);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription

        public bool Active { get; private set; }

        #endregion

        #region ISubscriptionState (Generic)

        IInvokableSingleArgGeneric<TValue> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.Invokable =>
            (IInvokableSingleArgGeneric<TValue>)invokable;

        IPoolElementFacade<ISubscription> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.PoolElement => poolElement;

        #endregion

        #region ISubscriptionState

        IInvokableSingleArg ISubscriptionState<IInvokableSingleArg>.Invokable =>
            (IInvokableSingleArg)invokable;

        IPoolElementFacade<ISubscription> ISubscriptionState<IInvokableSingleArg>.PoolElement => poolElement;

        #endregion

        #region ISubscriptionHandler (Generic)

        public bool ValidateActivation(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));

            if (poolElement != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));

            if (invokable == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID DELEGATE"));

            return true;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="poolElement">The pool element.</param>
        public void Activate(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher,
            IPoolElementFacade<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;

            Active = true;

            logger?.Log<SubscriptionSingleArgGeneric<TValue>>(
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }

        public bool ValidateTermination(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY INACTIVE"));

            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID PUBLISHER"));

            if (poolElement == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID POOL ELEMENT"));

            return true;
        }

        #endregion

        #region ISubscriptionHandler

        public bool ValidateActivation(
            INonAllocSubscribableSingleArg publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));

            if (poolElement != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));

            if (invokable == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID DELEGATE"));

            return true;
        }

        public void Activate(
            INonAllocSubscribableSingleArg publisher,
            IPoolElementFacade<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;

            Active = true;

            logger?.Log<SubscriptionSingleArgGeneric<TValue>>(
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }

        public bool ValidateTermination(
            INonAllocSubscribableSingleArg publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID PUBLISHER"));

            if (poolElement == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID POOL ELEMENT"));

            return true;
        }

        public void Terminate()
        {
            poolElement = null;

            publisher = null;

            Active = false;

            logger?.Log<SubscriptionSingleArgGeneric<TValue>>(
                $"SUBSCRIPTION TERMINATED: {this.GetHashCode()}");
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            //if (Active)
            //{
            //    switch (publisher)
            //    {
            //        case INonAllocSubscribableSingleArgGeneric<TValue> genericPublisher:
            //
            //            genericPublisher.Unsubscribe(this);
            //
            //            break;
            //
            //        case INonAllocSubscribableSingleArg nonGenericPublisher:
            //
            //            nonGenericPublisher.Unsubscribe<TValue>(this);
            //
            //            break;
            //    }
            //}

            Terminate();

            if (invokable is ICleanuppable)
                (invokable as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //if (Active)
            //{
            //    switch (publisher)
            //    {
            //        case INonAllocSubscribableSingleArgGeneric<TValue> genericPublisher:
            //
            //            genericPublisher.Unsubscribe(this);
            //
            //            break;
            //
            //        case INonAllocSubscribableSingleArg nonGenericPublisher:
            //        
            //            nonGenericPublisher.Unsubscribe<TValue>(this);
            //
            //            break;
            //    }
            //}

            Terminate();

            if (invokable is IDisposable)
                (invokable as IDisposable).Dispose();
        }

        #endregion
    }
}