using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionNoArgs
        : INonAllocSubscription,
          INonAllocSubscriptionContext<IInvokableNoArgs>,
          ICleanuppable,
          IDisposable
    {
        private readonly IInvokableNoArgs @delegate;

        private readonly ILogger logger;
        
        private INonAllocSubscribableNoArgs publisher;

        private IPoolElementFacade<ISubscription> poolElement;
        
        public SubscriptionNoArgs(
            Action @delegate,
            ILogger logger = null)
        {
            this.@delegate = DelegatesFactory.BuildDelegateWrapperNoArgs(
                @delegate);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        public bool Active { get; private set;  }

        public void Subscribe(
            INonAllocSubscribable publisher)
        {
            
        }

        public void Unsubscribe()
        {
        }


        #endregion

        #region ISubscriptionContext

        public IInvokableNoArgs Delegate
        {
            get => invokable;
        }

        public IPoolElementFacade<ISubscription> PoolElement
        {
            get => poolElement;
        }

        public bool ValidateActivation(
            INonAllocSubscribableNoArgs publisher)
        {
            if (Active)
            {
                logger?.LogError(
                    GetType(),
                    "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

                return false;
            }

            if (this.publisher != null)
            {
                logger?.LogError(
                    GetType(),
                    "SUBSCRIPTION ALREADY HAS A PUBLISHER");

                return false;
            }

            if (poolElement != null)
            {
                logger?.LogError(
                    GetType(),
                    "SUBSCRIPTION ALREADY HAS A POOL ELEMENT");

                return false;
            }

            if (@delegate == null)
            {
                logger?.LogError(
                    GetType(),
                    "INVALID DELEGATE");

                return false;
            }

            return true;
        }

        public void Activate(
            INonAllocSubscribableNoArgs publisher,
            IPoolElementFacade<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;

            Active = true;

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }

        public bool ValidateTermination(
            INonAllocSubscribableNoArgs publisher)
        {
            if (!Active)
            {
                logger?.LogError(
                    GetType(),
                    "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

                return false;
            }

            if (this.publisher != publisher)
            {
                logger?.LogError(
                    GetType(),
                    "INVALID PUBLISHER");

                return false;
            }

            if (poolElement == null)
            {
                logger?.LogError(
                    GetType(),
                    "INVALID POOL ELEMENT");

                return false;
            }

            return true;
        }

        public void Terminate()
        {
            poolElement = null;

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
            //if (Active)
            //    publisher.Unsubscribe(this);

            Terminate();

            if (invokable is ICleanuppable)
                (invokable as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //if (Active)
            //    publisher.Unsubscribe(this);

            Terminate();

            if (invokable is IDisposable)
                (invokable as IDisposable).Dispose();
        }

        #endregion
    }
}