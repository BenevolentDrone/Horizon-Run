using System;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
	public class ImmediateAsyncTransitionRequest<TBaseState>
		: AsyncTransitionRequest<TBaseState>
		where TBaseState : IState
	{
		private Type targetStateType;
		public Type TargetStateType
		{
			get
			{
				lock (lockObject)
				{
					return targetStateType;
				}
			}
			set
			{
				lock (lockObject)
				{
					targetStateType = value;
				}
			}
		}
	}
}