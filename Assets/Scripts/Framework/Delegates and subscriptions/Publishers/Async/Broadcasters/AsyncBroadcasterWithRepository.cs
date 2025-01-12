using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public class AsyncBroadcasterWithRepository
		: IAsyncPublisherSingleArg,
		  INonAllocSubscribable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IReadOnlyInstanceRepository broadcasterRepository;

		private readonly object lockObject;

		private readonly ILogger logger;

		public AsyncBroadcasterWithRepository(
			IReadOnlyInstanceRepository broadcasterRepository,
			ILogger logger = null)
		{
			this.broadcasterRepository = broadcasterRepository;

			this.logger = logger;


			lockObject = new object();
		}

		#region INonAllocSubscribable

		public bool Subscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				Type valueType = null;

				switch (subscription)
				{
					case INonAllocSubscriptionContext<IAsyncInvokableSingleArg> singleArgSubscriptionContext:

						valueType = singleArgSubscriptionContext.Delegate.ValueType;

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

						return false;
				}

				if (!broadcasterRepository.TryGet(
						valueType,
						out object broadcasterObject))
				{
					logger?.LogError(
						GetType(),
						$"INVALID VALUE TYPE: \"{valueType.Name}\"");

					return false;
				}

				var broadcaster = (INonAllocSubscribable)broadcasterObject;

				broadcaster.Subscribe(subscription);

				return true;
			}
		}

		public bool Unsubscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				Type valueType = null;

				switch (subscription)
				{
					case INonAllocSubscriptionContext<IAsyncInvokableSingleArg> singleArgSubscriptionContext:

						valueType = singleArgSubscriptionContext.Delegate.ValueType;

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

						return false;
				}

				if (!broadcasterRepository.TryGet(
					valueType,
					out object broadcasterObject))
				{
					logger?.LogError(
						GetType(),
						$"INVALID VALUE TYPE: \"{valueType.Name}\"");

					return false;
				}

				var broadcaster = (INonAllocSubscribable)broadcasterObject;

				broadcaster.Unsubscribe(subscription);

				return true;
			}
		}

		public IEnumerable<INonAllocSubscription> AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					// TODO: Consider yield instead
					List<INonAllocSubscription> result = new List<INonAllocSubscription>();

					foreach (var key in broadcasterRepository.Keys)
					{
						var broadcasterObject = broadcasterRepository.Get(key);

						var broadcaster = (INonAllocSubscribable)broadcasterObject;

						result.AddRange(broadcaster.AllSubscriptions);
					}

					return result;
				}
			}
		}

		public void UnsubscribeAll()
		{
			lock (lockObject)
			{
				foreach (var broadcaster in broadcasterRepository.Values)
				{
					((INonAllocSubscribable)broadcaster).UnsubscribeAll();
				}
			}
		}

		#endregion

		#region IAsyncPublisherSingleArg

		public async Task PublishAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncPublisherSingleArgGeneric<TValue> broadcaster = null;

			lock (lockObject)
			{
				var valueType = typeof(TValue);

				if (!broadcasterRepository.TryGet(
						valueType,
						out object broadcasterObject))
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID VALUE TYPE: \"{valueType.Name}\""));

				broadcaster = (IAsyncPublisherSingleArgGeneric<TValue>)broadcasterObject;
			}

			await broadcaster.PublishAsync(
				value,
				
				asyncContext);
		}

		public async Task PublishAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncPublisherSingleArg broadcaster = null;

			lock (lockObject)
			{
				if (!broadcasterRepository.TryGet(
						valueType,
						out object broadcasterObject))
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID VALUE TYPE: \"{valueType.Name}\""));

				broadcaster = (IAsyncPublisherSingleArg)broadcasterObject;
			}

			await broadcaster.PublishAsync(
				valueType,
				value,

				asyncContext);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				if (broadcasterRepository is ICleanuppable)
					(broadcasterRepository as ICleanuppable).Cleanup();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				if (broadcasterRepository is IDisposable)
					(broadcasterRepository as IDisposable).Dispose();
			}
		}

		#endregion
	}
}