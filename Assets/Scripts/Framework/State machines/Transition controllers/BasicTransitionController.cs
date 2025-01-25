using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public class BasicTransitionController<TBaseState>
		: ITransitionController<TBaseState>
		where TBaseState : IState
	{
		#region ITransitionController

		public void EnterState(
			TBaseState state)
		{
			state.EnterState();
		}

		public void EnterState(
			TBaseState state,
			ITransitionRequest transitionRequest)
		{
			transitionRequest.NextStateEnterProgress?.Report(0f);

			state.EnterState(
				transitionRequest);

			transitionRequest.NextStateEnterProgress?.Report(1f);

			var publisher = transitionRequest.OnNextStateEntered as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		public void ExitState(
			TBaseState state)
		{
			state.ExitState();
		}

		public void ExitState(
			TBaseState state,
			ITransitionRequest transitionRequest)
		{
			transitionRequest.PreviousStateExitProgress?.Report(0f);

			state.ExitState(
				transitionRequest);

			transitionRequest.PreviousStateExitProgress?.Report(1f);

			var publisher = transitionRequest.OnPreviousStateExited as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		#endregion
	}
}