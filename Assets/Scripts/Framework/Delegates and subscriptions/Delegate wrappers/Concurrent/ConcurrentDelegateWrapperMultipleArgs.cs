using System;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class ConcurrentDelegateWrapperMultipleArgs
		: IInvokableMultipleArgs
	{
		private readonly Action<object[]> @delegate;

		private readonly object lockObject;

		public ConcurrentDelegateWrapperMultipleArgs(
			Action<object[]> @delegate)
		{
			this.@delegate = @delegate;

			lockObject = new object();
		}

		public void Invoke(
			object[] arguments)
		{
			Action<object[]> copy;

			lock (lockObject)
			{
				copy = @delegate;  // Make a thread-safe copy of the delegate.
			}

			copy?.Invoke(arguments);  // Invoke the delegate outside of the lock.
		}
	}
}