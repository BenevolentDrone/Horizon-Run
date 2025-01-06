using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionMultipleArgs
        : ISubscription,
          ISubscriptionContext<IInvokableMultipleArgs>,
          ICleanuppable,
          IDisposable
    {
        private readonly IInvokableMultipleArgs invokable;

        private readonly ILogger logger;

        private INonAllocSubscribableMultipleArgs publisher;

        private IPoolElementFacade<ISubscription> poolElement;
        
        public SubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILogger logger = null)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperMultipleArgs(@delegate);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        public bool Active { get; private set;  }

        #endregion
        
        #region ISubscriptionState

        public IInvokableMultipleArgs Invokable
        {
            get => invokable;
        }

        public IPoolElementFacade<ISubscription> PoolElement
        {
            get => poolElement;
        }

        #endregion

        #region ISubscriptionHandler
        
        public bool ValidateActivation(INonAllocSubscribableMultipleArgs publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));
			
            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));
			
            if (poolElement != null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));
			
            if (invokable == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "INVALID DELEGATE"));

            return true;
        }
        
        public void Activate(
            INonAllocSubscribableMultipleArgs publisher,
            IPoolElementFacade<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;

            logger?.Log<SubscriptionMultipleArgs>(
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }
        
        public bool ValidateTermination(INonAllocSubscribableMultipleArgs publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));
			
            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "INVALID PUBLISHER"));
			
            if (poolElement == null)
                throw new Exception(
                    logger.TryFormatException<SubscriptionMultipleArgs>(
                        "INVALID POOL ELEMENT"));

            return true;
        }
        
        public void Terminate()
        {
            poolElement = null;
            
            publisher = null;
            
            Active = false;

            logger?.Log<SubscriptionMultipleArgs>(
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