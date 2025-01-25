using System;

namespace HereticalSolutions.StateMachines
{
	public class ImmediateTransitionRequest
		: ATransitionRequest
	{
		private Type targetStateType;

		public Type TargetStateType
		{
			get => targetStateType;
			set => targetStateType = value;
		}
	}
}