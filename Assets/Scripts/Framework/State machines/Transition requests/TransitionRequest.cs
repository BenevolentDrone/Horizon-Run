using System;

namespace HereticalSolutions.StateMachines
{
	public class TransitionRequest<TBaseState>
		where TBaseState : IState
	{
		public ETransitionState TransitionState = ETransitionState.UNINITIALISED;


		public Action<TBaseState> OnPreviousStateExited { get; set; }

		public Action<TBaseState> OnNextStateEntered { get; set; }


		public IProgress<float> PreviousStateExitProgress;

		public IProgress<float> NextStateEnterProgress;
	}
}