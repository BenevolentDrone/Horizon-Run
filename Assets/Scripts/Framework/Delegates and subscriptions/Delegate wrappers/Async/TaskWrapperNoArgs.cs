using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class TaskWrapperNoArgs
		: IAsyncInvokableNoArgs
	{
		private readonly Func<AsyncExecutionContext, Task> taskFactory;

		public TaskWrapperNoArgs(
			Func<AsyncExecutionContext, Task> taskFactory)
		{
			this.taskFactory = taskFactory;
		}

		public async Task InvokeAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await taskFactory(
				asyncContext);
		}
	}
}