using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class TaskWrapperNoArgs
		: IAsyncInvokableNoArgs
	{
		private readonly Func<CancellationToken, IProgress<float>, ILogger, Task> taskFactory;

		public TaskWrapperNoArgs(
			Func<CancellationToken, IProgress<float>, ILogger, Task> taskFactory)
		{
			this.taskFactory = taskFactory;
		}

		public async Task InvokeAsync(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			await taskFactory(
				cancellationToken,
				progress,
				progressLogger);
		}
	}
}