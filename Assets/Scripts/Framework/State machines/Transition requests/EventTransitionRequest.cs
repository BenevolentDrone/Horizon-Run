namespace HereticalSolutions.StateMachines
{
	public class EventTransitionRequest<TBaseState>
		: TransitionRequest<TBaseState>
		where TBaseState : IState
	{
		public ITransitionEvent<TBaseState> Event;
	}
}