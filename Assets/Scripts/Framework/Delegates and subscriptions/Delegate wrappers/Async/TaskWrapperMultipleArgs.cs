using System;
using System.Threading;
using System.Threading.Tasks;

 using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class TaskWrapperMultipleArgs
		: IAsyncInvokableMultipleArgs
	{
		private readonly Func<object[], CancellationToken, IProgress<float>, ILogger, Task> taskFactory;

		public TaskWrapperMultipleArgs(
			Func<object[], CancellationToken, IProgress<float>, ILogger, Task> taskFactory)
		{
			this.taskFactory = taskFactory;
		}

		public async Task InvokeAsync(
			object[] arguments,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			await taskFactory(
				arguments,
				cancellationToken,
				progress,
				progressLogger);
		}
	}
}