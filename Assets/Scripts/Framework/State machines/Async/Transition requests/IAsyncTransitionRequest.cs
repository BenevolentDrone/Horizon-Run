using System;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public interface IAsyncTransitionRequest
	{
		public ETransitionState TransitionState { get; set; }

		public EAsyncTransitionRules Rules { get; set; }


		public bool CommencePreviousStateExitStart { get; set; }

		public INonAllocSubscribable OnPreviousStateExited { get; set; }

		public bool CommencePreviousStateExitFinish { get; set; }


		public bool CommenceNextStateEnterStart { get; set; }

		public INonAllocSubscribable OnNextStateEntered { get; set; }

		public bool CommenceNextStateEnterFinish { get; set; }


		public IProgress<float> PreviousStateExitProgress { get; set; }

		public IProgress<float> NextStateEnterProgress { get; set; }


		public AsyncExecutionContext AsyncContext { get; set; }
	}
}