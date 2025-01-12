using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
	public class ConcurrentSubscriptionMultipleArgs
		: INonAllocSubscription,
		  INonAllocSubscriptionContext<IInvokableMultipleArgs>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IInvokableMultipleArgs @delegate;

		private readonly object lockObject;

		private readonly ILogger logger;


		private INonAllocSubscribable publisher;

		private bool active;

		public ConcurrentSubscriptionMultipleArgs(
			Action<object[]> @delegate,
			ILogger logger = null)
		{
			this.@delegate = DelegateWrapperFactory.BuildDelegateWrapperMultipleArgs(
				@delegate);

			this.logger = logger;


			lockObject = new object();


			active = false;

			publisher = null;
		}

		#region INonAllocSubscription

		public bool Active
		{
			get
			{
				lock (lockObject)
				{
					return active;
				}
			}
		}

		public bool Subscribe(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				if (active)
					return false;
	
				return publisher.Subscribe(this);
			}
		}

		public bool Unsubscribe()
		{
			lock (lockObject)
			{
				if (!active)
					return false;
	
				return publisher.Unsubscribe(this);
			}
		}


		#endregion

		#region INonAllocSubscriptionContext

		public IInvokableMultipleArgs Delegate
		{
			get
			{
				lock (lockObject)
				{
					return @delegate;
				}
			}
		}

		public bool ValidateActivation(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				if (active)
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
	
				if (publisher is not IPublisherMultipleArgs
					&& publisher is not IAsyncPublisherMultipleArgs)
				{
					logger?.LogError(
						GetType(),
						$"INVALID PUBLISHER: EXPECTED {nameof(IPublisherMultipleArgs)} OR {nameof(IAsyncPublisherMultipleArgs)} : {this.GetHashCode()}");
	
					return false;
				}
	
				return true;
			}
		}

		public void Activate(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				this.publisher = publisher;
	
				active = true;
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
			}
		}

		public bool ValidateTermination(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				if (!active)
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
		}

		public void Terminate()
		{
			lock (lockObject)
			{
				publisher = null;
	
				active = false;
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION TERMINATED: {this.GetHashCode()}");
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				if (active
					&& publisher != null
					&& publisher.Unsubscribe(this))
				{
				}
				else
					Terminate();
	
				if (@delegate is ICleanuppable)
					(@delegate as ICleanuppable).Cleanup();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				if (active
					&& publisher != null
					&& publisher.Unsubscribe(this))
				{
				}
				else
					Terminate();
	
				if (@delegate is IDisposable)
					(@delegate as IDisposable).Dispose();
			}
		}

		#endregion
	}
}