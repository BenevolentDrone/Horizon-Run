using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public class ConcurrentBroadcasterGeneric<TValue>
		: IPublisherSingleArgGeneric<TValue>,
		  IPublisherSingleArg,
		  ISubscribableSingleArgGeneric<TValue>,
		  ISubscribableSingleArg,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IPool<BroadcasterGenericInvocationContext<TValue>> contextPool;

		private readonly object lockObject;

		private readonly ILogger logger;

		private Action<TValue> multicastDelegate;

		public ConcurrentBroadcasterGeneric(
			IPool<BroadcasterGenericInvocationContext<TValue>> contextPool,
			ILogger logger = null)
		{
			this.contextPool = contextPool;

			this.logger = logger;


			lockObject = new object();


			multicastDelegate = null;
		}

		#region IPublisherSingleArg

		public void Publish<TArgument>(
			TArgument value)
		{
			switch (value)
			{
				case TValue tValue:

					lock (lockObject)
					{
						multicastDelegate?.Invoke(tValue);
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
		}

		public void Publish(
			Type valueType,
			object value)
		{
			switch (value)
			{
				case TValue tValue:

					lock (lockObject)
					{
						multicastDelegate?.Invoke(tValue);
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		#endregion

		#region ISubscribableSingleArgGeneric

		public void Subscribe(
			Action<TValue> @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate += @delegate;
			}
		}

		public void Subscribe(
			object @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate += (Action<TValue>)@delegate;
			}
		}

		public void Unsubscribe(
			Action<TValue> @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate -= @delegate;
			}
		}

		public void Unsubscribe(
			object @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate -= (Action<TValue>)@delegate;
			}
		}

		IEnumerable<Action<TValue>> ISubscribableSingleArgGeneric<TValue>.AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					return multicastDelegate?
						.GetInvocationList()
						.CastInvocationListToGenericActions<TValue>()
						?? new Action<TValue>[0];
				}
			}
		}

		#endregion

		#region ISubscribableSingleArg

		public void Subscribe<TArgument>(
			Action<TArgument> @delegate)
		{
			switch (@delegate)
			{
				case Action<TValue> tValue:

					lock (lockObject)
					{
						multicastDelegate += tValue;
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
		}

		public void Subscribe(
			Type valueType,
			object @delegate)
		{
			switch (@delegate)
			{
				case Action<TValue> tValue:

					lock (lockObject)
					{
						multicastDelegate += tValue;
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		public void Unsubscribe<TArgument>(
			Action<TArgument> @delegate)
		{
			switch (@delegate)
			{
				case Action<TValue> tValue:

					lock (lockObject)
					{
						multicastDelegate -= tValue;
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
		}

		public void Unsubscribe(
			Type valueType,
			object @delegate)
		{
			switch (@delegate)
			{
				case Action<TValue> tValue:

					lock (lockObject)
					{
						multicastDelegate -= tValue;
					}

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		public IEnumerable<Action<TArgument>> GetAllSubscriptions<TArgument>()
		{
			lock (lockObject)
			{
				return multicastDelegate?
					.GetInvocationList()
					.CastInvocationListToGenericActions<TArgument>()
					?? new Action<TArgument>[0];
			}
		}

		public IEnumerable<object> GetAllSubscriptions(
			Type valueType)
		{
			lock (lockObject)
			{
				return multicastDelegate?
					.GetInvocationList()
					.CastInvocationListToObjects()
					?? new object[0];
			}
		}

		#endregion

		#region ISubscribable

		IEnumerable<object> ISubscribable.AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					return multicastDelegate?
						.GetInvocationList()
						.CastInvocationListToObjects()
						?? new object[0];
				}
			}
		}

		public void UnsubscribeAll()
		{
			lock (lockObject)
			{
				multicastDelegate = null;
			}
		}

		#endregion

		#region IPublisherSingleArgGeneric

		public void Publish(
			TValue value)
		{
			lock (lockObject)
			{
				var context = contextPool.Pop();

				context.Delegate = multicastDelegate;

				context.Delegate?.Invoke(value);

				context.Delegate = null;

				contextPool.Push(context);
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				multicastDelegate = null;
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				multicastDelegate = null;
			}
		}

		#endregion
	}
}