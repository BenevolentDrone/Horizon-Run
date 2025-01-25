using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public abstract class ATransitionRequest
	{
		protected ETransitionState transitionState = ETransitionState.UNINITIALISED;

		protected INonAllocSubscribable onPreviousStateExited;

		protected INonAllocSubscribable onNextStateEntered;


		protected IProgress<float> previousStateExitProgress;

		protected IProgress<float> nextStateEnterProgress;

		#region ITransitionRequest

		public ETransitionState TransitionState
		{
			get => transitionState;
			set => transitionState = value;
		}


		public INonAllocSubscribable OnPreviousStateExited
		{
			get => onPreviousStateExited;
			set => onPreviousStateExited = value;
		}

		public INonAllocSubscribable OnNextStateEntered
		{
			get => onNextStateEntered;
			set => onNextStateEntered = value;
		}

		//Action<TBaseState> OnPreviousStateExited { get; set; }

		//Action<TBaseState> OnNextStateEntered { get; set; }


		public IProgress<float> PreviousStateExitProgress
		{
			get => previousStateExitProgress;
			set => previousStateExitProgress = value;
		}

		public IProgress<float> NextStateEnterProgress
		{
			get => nextStateEnterProgress;
			set => nextStateEnterProgress = value;
		}

		#endregion
	}
}