using System;
using System.Collections.Generic;

using HereticalSolutions.Pools;

using HereticalSolutions.Bags;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public class ConcurrentNonAllocBroadcasterGeneric<TValue>
		: IPublisherSingleArgGeneric<TValue>,
		  IPublisherSingleArg,
		  INonAllocSubscribable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IBag<INonAllocSubscription> subscriptionsBag;

		private readonly IPool<NonAllocBroadcasterInvocationContext> contextPool;

		private readonly object lockObject;

		private readonly ILogger logger;

		public ConcurrentNonAllocBroadcasterGeneric(
			IBag<INonAllocSubscription> subscriptionsBag,
			IPool<NonAllocBroadcasterInvocationContext> contextPool,
			ILogger logger)
		{
			this.subscriptionsBag = subscriptionsBag;

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
				switch (subscription)
				{
					case INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>
						singleArgGenericSubscriptionContext:
						
						if (!singleArgGenericSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionsBag.Push(subscription))
							return false;

						singleArgGenericSubscriptionContext.Activate(this);

						break;

					case INonAllocSubscriptionContext<IInvokableSingleArg> singleArgSubscriptionContext:
						
						if (!singleArgSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionsBag.Push(subscription))
							return false;

						singleArgSubscriptionContext.Activate(this);

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

						return false;
				}

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
				switch (subscription)
				{
					case INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>
						singleArgGenericSubscriptionContext:
						
						if (!singleArgGenericSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionsBag.Pop(subscription))
							return false;

						singleArgGenericSubscriptionContext.Terminate();
						
						break;

					case INonAllocSubscriptionContext<IInvokableSingleArg> singleArgSubscriptionContext:

						if (!singleArgSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionsBag.Pop(subscription))
							return false;

						singleArgSubscriptionContext.Terminate();

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");
						
						return false;
				}

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
					return subscriptionsBag.All;
				}
			}
		}

		public void UnsubscribeAll()
		{
			lock (lockObject)
			{
				foreach (var subscription in subscriptionsBag.All)
				{
					switch (subscription)
					{
						case INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>
							singleArgGenericSubscriptionContext:

							if (!singleArgGenericSubscriptionContext.ValidateActivation(this))
								continue;

							singleArgGenericSubscriptionContext.Terminate();

							break;

						case INonAllocSubscriptionContext<IInvokableSingleArg> singleArgSubscriptionContext:

							if (!singleArgSubscriptionContext.ValidateActivation(this))
								continue;

							singleArgSubscriptionContext.Terminate();

							break;

						default:
							continue;
					}
				}

				subscriptionsBag.Clear();
			}
		}

		#endregion

		#region IPublisherSingleArgGeneric

		public void Publish(
			TValue value)
		{
			NonAllocBroadcasterInvocationContext context = null;

			int count = -1;

			lock (lockObject)
			{
				if (subscriptionsBag.Count == 0)
					return;

				// Pop context out of the pool and initialize it with values from the bag

				count = subscriptionsBag.Count;

				context = contextPool.Pop();

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
			}

			// Invoke the delegates in the context

			for (int i = 0; i < context.Count; i++)
			{
				var subscriptionContext = context.Subscriptions[i] as INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>;

				if (subscriptionContext == null)
					continue;

				subscriptionContext.Delegate?.Invoke(value);
			}

			lock (lockObject)
			{
				// Clean up and push the context back into the pool

				for (int i = 0; i < count; i++)
				{
					context.Subscriptions[i] = null;
				}

				context.Count = 0;

				contextPool.Push(context);
			}
		}

		#endregion

		#region IPublisherSingleArg

		public void Publish<TArgument>(
			TArgument value)
		{
			//lock (lockObject)
			//{
				switch (value)
				{
					case TValue tValue:

						Publish(tValue);

						break;

					default:

						throw new Exception(
							logger.TryFormatException(
								GetType(),
								$"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{nameof(TArgument)}\""));
				}
			//}
		}

		public void Publish(
			Type valueType,
			object value)
		{
			//lock (lockObject)
			//{
				switch (value)
				{
					case TValue tValue:

						Publish(tValue);

						break;

					default:
						throw new Exception(
							logger.TryFormatException(
								GetType(),
								$"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{valueType.Name}\""));
				}
			//}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				if (subscriptionsBag is ICleanuppable)
					(subscriptionsBag as ICleanuppable).Cleanup();

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
				if (subscriptionsBag is IDisposable)
					(subscriptionsBag as IDisposable).Dispose();

				if (contextPool is IDisposable)
					(contextPool as IDisposable).Dispose();
			}
		}

		#endregion
	}
}