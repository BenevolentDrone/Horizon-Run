using System;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class ConcurrentDelegateWrapperNoArgs
		: IInvokableNoArgs
	{
		private readonly Action @delegate;

		private readonly object lockObject;

		public ConcurrentDelegateWrapperNoArgs(
			Action @delegate)
		{
			this.@delegate = @delegate;

			lockObject = new object();
		}

		public void Invoke()
		{
			Action copy;

			lock (lockObject)
			{
				copy = @delegate;  // Make a thread-safe copy of the delegate.
			}

			copy?.Invoke();  // Invoke the delegate outside of the lock.
		}
	}
}