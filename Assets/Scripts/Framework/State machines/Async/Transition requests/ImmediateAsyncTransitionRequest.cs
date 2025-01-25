using System;

namespace HereticalSolutions.StateMachines
{
	public class ImmediateAsyncTransitionRequest
		: AAsyncTransitionRequest
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