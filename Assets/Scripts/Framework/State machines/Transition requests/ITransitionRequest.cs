using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public interface ITransitionRequest
	{
		ETransitionState TransitionState { get; set; }


		INonAllocSubscribable OnPreviousStateExited { get; }

		INonAllocSubscribable OnNextStateEntered { get; }


		IProgress<float> PreviousStateExitProgress { get; set; }

		IProgress<float> NextStateEnterProgress { get; set; }
	}
}