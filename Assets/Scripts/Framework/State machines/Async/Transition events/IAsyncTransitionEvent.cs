namespace HereticalSolutions.StateMachines
{
	public interface IAsyncTransitionEvent<TBaseState>
		where TBaseState : IAsyncState
	{
		TBaseState From { get; }

		TBaseState To { get; }

		EAsyncTransitionRules Rules { get; }
	}
}