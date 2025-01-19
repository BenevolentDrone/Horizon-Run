using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Pools;

using HereticalSolutions.Bags;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public class AsyncPinger
		: IAsyncPublisherNoArgs,
		  INonAllocSubscribable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IBag<INonAllocSubscription> subscriptionBag;

		private readonly IPool<NonAllocPingerInvocationContext> contextPool;

		private readonly object lockObject;

		private readonly ILogger logger;

		public AsyncPinger(
			IBag<INonAllocSubscription> subscriptionBag,
			IPool<NonAllocPingerInvocationContext> contextPool,
			ILogger logger)
		{
			this.subscriptionBag = subscriptionBag;

			this.contextPool = contextPool;

			this.logger = logger;


			lockObject = new object();
		}

		#region INonAllocSubscribable

		public bool Subscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableNoArgs>;
	
				if (subscriptionContext == null)
					return false;
	
				if (!subscriptionContext.ValidateActivation(this))
					return false;
	
				if (!subscriptionBag.Push(
					subscription))
					return false;
	
				subscriptionContext.Activate(
					this);
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION {subscription.GetHashCode()} ADDED: {this.GetHashCode()}");
	
				return true;
			}
		}

		public bool Unsubscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableNoArgs>;
	
				if (subscriptionContext == null)
					return false;
	
				if (!subscriptionContext.ValidateActivation(this))
					return false;
	
				if (!subscriptionBag.Pop(
					subscription))
					return false;
	
				subscriptionContext.Terminate();
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION {subscription.GetHashCode()} REMOVED: {this.GetHashCode()}");
	
				return true;
			}
		}

		public IEnumerable<INonAllocSubscription> AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					return subscriptionBag.All;
				}
			}
		}

		public void UnsubscribeAll()
		{
			lock (lockObject)
			{
				foreach (var subscription in subscriptionBag.All)
				{
					var subscriptionContext = subscription as INonAllocSubscriptionContext<IInvokableNoArgs>;
	
					if (subscriptionContext == null)
						continue;
	
					if (!subscriptionContext.ValidateActivation(this))
						continue;
	
					subscriptionContext.Terminate();
				}
	
				subscriptionBag.Clear();
			}
		}

		#endregion

		#region IAsyncPublisherNoArgs

		public async Task PublishAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			NonAllocPingerInvocationContext context = null;

			int count = -1;

			lock (lockObject)
			{
				if (subscriptionBag.Count == 0)
					return;
	
				//Pop context out of the pool and initialize it with values from the bag
	
				count = subscriptionBag.Count;
	
				context = contextPool.Pop();
	
				bool newBuffer = false;
	
				if (context.Subscriptions == null)
				{
					context.Subscriptions = new INonAllocSubscription[count];
	
					newBuffer = true;
				}
	
				if (context.Subscriptions.Length < subscriptionBag.Count)
				{
					context.Subscriptions = new INonAllocSubscription[subscriptionBag.Count];
	
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
				foreach (var subscription in subscriptionBag.All)
				{
					context.Subscriptions[index] = subscription;
	
					index++;
				}
	
				context.Count = count;
			}
	
	
			//Invoke the delegates in the context

			for (int i = 0; i < context.Count; i++)
			{
				var subscriptionContext = context.Subscriptions[i] as INonAllocSubscriptionContext<IAsyncInvokableNoArgs>;

				if (subscriptionContext == null)
					continue;

				await subscriptionContext.Delegate?.InvokeAsync(
					
					asyncContext);
			}
	
			lock (lockObject)
			{
				//Clean up and push the context back into the pool
	
				for (int i = 0; i < count; i++)
				{
					context.Subscriptions[i] = null;
				}
	
				context.Count = 0;
	
				contextPool.Push(context);
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				if (subscriptionBag is ICleanuppable)
					(subscriptionBag as ICleanuppable).Cleanup();
	
				if (contextPool is ICleanuppable)
					(contextPool as ICleanuppable).Cleanup();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				if (subscriptionBag is IDisposable)
					(subscriptionBag as IDisposable).Dispose();
	
				if (contextPool is IDisposable)
					(contextPool as IDisposable).Dispose();
			}
		}

		#endregion
	}
}