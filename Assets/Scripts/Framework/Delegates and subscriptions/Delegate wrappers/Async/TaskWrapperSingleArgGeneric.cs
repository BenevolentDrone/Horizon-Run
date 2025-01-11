using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class TaskWrapperSingleArgGeneric<TValue>
		: IAsyncInvokableSingleArgGeneric<TValue>,
		  IAsyncInvokableSingleArg
	{
		private readonly Func<TValue, CancellationToken, IProgress<float>, ILogger, Task> taskFactory;

		private readonly ILogger logger;

		public TaskWrapperSingleArgGeneric(
			Func<TValue, CancellationToken, IProgress<float>, ILogger, Task> taskFactory,
			ILogger logger = null)
		{
			this.taskFactory = taskFactory;

			this.logger = logger;
		}

		#region IAsyncInvokableSingleArgGeneric

		public async Task InvokeAsync(
			TValue argument,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			await taskFactory(
				argument,
				cancellationToken,
				progress,
				progressLogger);
		}

		public async Task InvokeAsync(
			object argument,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			switch (argument)
			{
				case TValue tValue:

					await taskFactory(
						tValue,
						cancellationToken,
						progress,
						progressLogger);

						break;
				
				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{argument.GetType().Name}\""));
			}
		}

		#endregion

		#region IAsyncInvokableSingleArg

		public Type ValueType => typeof(TValue);

		public async Task InvokeAsync<TArgument>(
			TArgument value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			switch (value)
			{
				case TValue tValue:

					await taskFactory(
						tValue,
						cancellationToken,
						progress,
						progressLogger);

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
		}

		public async Task InvokeAsync(
			Type valueType,
			object value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			switch (value)
			{
				case TValue tValue:

					await taskFactory(
						tValue,
						cancellationToken,
						progress,
						progressLogger);

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		#endregion
	}
}