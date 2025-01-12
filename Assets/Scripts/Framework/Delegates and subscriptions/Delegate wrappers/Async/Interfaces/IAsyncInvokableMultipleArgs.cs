using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncInvokableMultipleArgs
	{
		Task InvokeAsync(
			object[] args,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}