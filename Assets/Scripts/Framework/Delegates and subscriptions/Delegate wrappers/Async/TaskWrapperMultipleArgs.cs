using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.Wrappers
{
	public class TaskWrapperMultipleArgs
		: IAsyncInvokableMultipleArgs
	{
		private readonly Func<object[], AsyncExecutionContext, Task> taskFactory;

		public TaskWrapperMultipleArgs(
			Func<object[], AsyncExecutionContext, Task> taskFactory)
		{
			this.taskFactory = taskFactory;
		}

		public async Task InvokeAsync(
			object[] arguments,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await taskFactory(
				arguments,
				
				asyncContext);
		}
	}
}