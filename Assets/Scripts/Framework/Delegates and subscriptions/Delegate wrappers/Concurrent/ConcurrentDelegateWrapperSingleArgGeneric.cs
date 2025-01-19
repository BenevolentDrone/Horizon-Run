using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class ConcurrentDelegateWrapperSingleArgGeneric<TValue>
		: IInvokableSingleArgGeneric<TValue>,
		  IInvokableSingleArg
	{
		private readonly Action<TValue> @delegate;

		private readonly object lockObject;

		private readonly ILogger logger;

		public ConcurrentDelegateWrapperSingleArgGeneric(
			Action<TValue> @delegate,
			ILogger logger)
		{
			this.@delegate = @delegate;

			this.logger = logger;


			lockObject = new object();
		}

		#region IInvokableSingleArgGeneric

		public Type ValueType => typeof(TValue);

		public void Invoke(
			TValue argument)
		{
			Action<TValue> copy;

			lock (lockObject)
			{
				copy = @delegate;  // Make a thread-safe copy of the delegate.
			}

			copy?.Invoke(argument);  // Invoke the delegate outside of the lock.
		}

		public void Invoke(
			object argument)
		{
			switch (argument)
			{
				case TValue tValue:

					Action<TValue> copy;

					lock (lockObject)
					{
						copy = @delegate;  // Make a thread-safe copy of the delegate.
					}

					copy?.Invoke(tValue);  // Invoke the delegate outside of the lock.

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{argument.GetType().Name}\""));
			}
		}

		#endregion

		#region IInvokableSingleArg

		public void Invoke<TArgument>(
			TArgument value)
		{
			switch (value)
			{
				case TValue tValue:

					Action<TValue> copy;

					lock (lockObject)
					{
						copy = @delegate;  // Make a thread-safe copy of the delegate.
					}

					copy?.Invoke(tValue);  // Invoke the delegate outside of the lock.

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{nameof(TArgument)}\""));
			}
		}

		public void Invoke(
			Type valueType,
			object value)
		{
			switch (value)
			{
				case TValue tValue:

					Action<TValue> copy;

					lock (lockObject)
					{
						copy = @delegate;  // Make a thread-safe copy of the delegate.
					}

					copy?.Invoke(tValue);  // Invoke the delegate outside of the lock.

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		#endregion
	}
}