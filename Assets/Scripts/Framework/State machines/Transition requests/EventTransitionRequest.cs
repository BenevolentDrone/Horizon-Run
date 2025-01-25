namespace HereticalSolutions.StateMachines
{
	public class EventTransitionRequest<TBaseState>
		: ATransitionRequest
		where TBaseState : IState
	{
		private ITransitionEvent<TBaseState> @event;

		public ITransitionEvent<TBaseState> Event
		{
			get => @event;
			set => @event = value;
		}
	}
}