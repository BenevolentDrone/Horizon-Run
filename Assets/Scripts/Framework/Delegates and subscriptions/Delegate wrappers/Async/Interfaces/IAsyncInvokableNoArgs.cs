using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncInvokableNoArgs
	{
		Task InvokeAsync(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}