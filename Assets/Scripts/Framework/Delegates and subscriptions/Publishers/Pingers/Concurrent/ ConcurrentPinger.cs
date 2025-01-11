using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
	public class ConcurrentPinger
		: IPublisherNoArgs,
		  ISubscribableNoArgs,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IPool<PingerInvocationContext> contextPool;

		private readonly object lockObject;

		private Action multicastDelegate;

		public ConcurrentPinger(
			IPool<PingerInvocationContext> contextPool)
		{
			this.contextPool = contextPool;

			lockObject = new object();

			multicastDelegate = null;
		}

		#region ISubscribableNoArgs

		public void Subscribe(
			Action @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate += @delegate;
			}
		}

		public void Unsubscribe(
			Action @delegate)
		{
			lock (lockObject)
			{
				multicastDelegate -= @delegate;
			}
		}

		IEnumerable<Action> ISubscribableNoArgs.AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					//Kudos to Copilot for Cast() and the part after the ?? operator
					return multicastDelegate?
						.GetInvocationList()
						.CastInvocationListToActions()
						?? new Action[0];
				}
			}
		}

		#region ISubscribable

		IEnumerable<object> ISubscribable.AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					//Kudos to Copilot for Cast() and the part after the ?? operator
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

		#endregion

		#region IPublisherNoArgs

		public void Publish()
		{
			PingerInvocationContext context = null;

			lock (lockObject)
			{
				context = contextPool.Pop();
	
				context.Delegate = multicastDelegate;
			}
	
			context.Delegate?.Invoke();
	
			lock (lockObject)
			{
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