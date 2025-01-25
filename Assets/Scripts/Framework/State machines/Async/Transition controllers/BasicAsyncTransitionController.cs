using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public class BasicAsyncTransitionController<TBaseState>
		: IAsyncTransitionController<TBaseState>
		where TBaseState : IAsyncState
	{
		#region IAsyncTransitionController

		public async Task EnterState(
			TBaseState state,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await state.EnterState(
				asyncContext);
		}

		public async Task EnterState(
			TBaseState state,
			IAsyncTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			transitionRequest.NextStateEnterProgress?.Report(0f);

			await state.EnterState(
				transitionRequest,
				
				asyncContext);

			transitionRequest.NextStateEnterProgress?.Report(1f);

			var publisher = transitionRequest.OnNextStateEntered as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		public async Task ExitState(
			TBaseState state,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await state.ExitState(
				asyncContext);
		}

		public async Task ExitState(
			TBaseState state,
			IAsyncTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			transitionRequest.PreviousStateExitProgress?.Report(0f);

			await state.ExitState(
				transitionRequest,
				
				asyncContext);

			transitionRequest.PreviousStateExitProgress?.Report(1f);

			var publisher = transitionRequest.OnPreviousStateExited as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		#endregion
	}
}