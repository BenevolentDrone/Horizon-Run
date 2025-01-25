namespace HereticalSolutions.StateMachines
{
	public interface ITransitionController<TBaseState>
		where TBaseState : IState
	{
		void EnterState(
			TBaseState state);

		void EnterState(
			TBaseState state,
			ITransitionRequest transitionRequest);

		void ExitState(
			TBaseState state);

		void ExitState(
			TBaseState state,
			ITransitionRequest transitionRequest);
	}
}