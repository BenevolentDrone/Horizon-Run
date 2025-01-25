using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
	public interface IAsyncState
	{
		Task EnterState(
			
			//Async tail
			AsyncExecutionContext asyncContext);

		Task EnterState(
			IAsyncTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task ExitState(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task ExitState(
			IAsyncTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}