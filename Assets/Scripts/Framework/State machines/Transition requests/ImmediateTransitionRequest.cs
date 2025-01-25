using System;

namespace HereticalSolutions.StateMachines
{
	public class ImmediateTransitionRequest<TBaseState>
		: TransitionRequest<TBaseState>
		where TBaseState : IState
	{
		public Type TargetStateType;
	}
}