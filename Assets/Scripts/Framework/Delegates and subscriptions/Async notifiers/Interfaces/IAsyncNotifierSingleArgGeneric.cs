using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Notifiers
{
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		Task<TValue> GetValueWhenNotified(
			TArgument argument = default,
			bool ignoreKey = false,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<Task<TValue>> GetWaitForNotificationTask(
			TArgument argument = default,
			bool ignoreKey = false,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task Notify(
			TArgument argument,
			TValue value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}