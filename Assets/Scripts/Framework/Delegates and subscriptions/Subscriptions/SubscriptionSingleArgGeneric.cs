using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionSingleArgGeneric<TValue>
        : INonAllocSubscription,
          INonAllocSubscriptionContext<IInvokableSingleArg>,
          INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>,
          ICleanuppable,
          IDisposable
    {
        private readonly IInvokableSingleArgGeneric<TValue> @delegate;

        private readonly ILogger logger;

        private INonAllocSubscribable publisher;

        public SubscriptionSingleArgGeneric(
            Action<TValue> @delegate,
            ILogger logger)
        {
            this.@delegate = DelegateWrapperFactory.BuildDelegateWrapperSingleArgGeneric(
                @delegate);

            this.logger = logger;

            Active = false;

            publisher = null;
        }

        #region INonAllocSubscription

        public bool Active { get; private set; }

        public bool Subscribe(
            INonAllocSubscribable publisher)
        {
            if (Active)
                return false;

            return publisher.Subscribe(this);
        }

        public bool Unsubscribe()
        {
            if (!Active)
                return false;

            return publisher.Unsubscribe(this);
        }


        #endregion

        #region INonAllocSubscriptionContext

        IInvokableSingleArg INonAllocSubscriptionContext<IInvokableSingleArg>.Delegate
        {
            get => (IInvokableSingleArg)@delegate;
        }

        IInvokableSingleArgGeneric<TValue> INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>.Delegate
        {
            get => @delegate;
        }

        public bool ValidateActivation(
            INonAllocSubscribable publisher)
        {
            if (Active)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE: {this.GetHashCode()}");

                return false;
            }

            if (this.publisher != null)
            {
                logger?.LogError(
                    GetType(),
                    $"SUBSCRIPTION ALREADY HAS A PUBLISHER: {this.GetHashCode()}");

                return false;
            }

            if (@delegate == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID DELEGATE: {this.GetHashCode()}");

                return false;
            }

            if (publisher is not IPublisherSingleArg
                && publisher is not IPublisherSingleArgGeneric<TValue>
                && publisher is not IAsyncPublisherSingleArg
                && publisher is not IAsyncPublisherSingleArgGeneric<TValue>)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PUBLISHER: EXPECTED {nameof(IPublisherSingleArg)} OR {nameof(IPublisherSingleArgGeneric<TValue>)} OR {nameof(IAsyncPublisherSingleArg)} OR {nameof(IAsyncPublisherSingleArgGeneric<TValue>)} : {this.GetHashCode()}");

                return false;
            }

            return true;
        }

        public void Activate(
            INonAllocSubscribable publisher)
        {
            this.publisher = publisher;

            Active = true;

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }

        public bool ValidateTermination(
            INonAllocSubscribable publisher)
        {
            if (!Active)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE: {this.GetHashCode()}");

                return false;
            }

            if (this.publisher != publisher)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PUBLISHER: {this.GetHashCode()}");

                return false;
            }

            return true;
        }

        public void Terminate()
        {
            publisher = null;

            Active = false;

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION TERMINATED: {this.GetHashCode()}");
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (Active
                && publisher != null
                && publisher.Unsubscribe(this))
            {
            }
            else
                Terminate();

            if (@delegate is ICleanuppable)
                (@delegate as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (Active
                && publisher != null
                && publisher.Unsubscribe(this))
            {
            }
            else
                Terminate();

            if (@delegate is IDisposable)
                (@delegate as IDisposable).Dispose();
        }

        #endregion
    }
}