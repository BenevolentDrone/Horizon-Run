using System;

namespace HereticalSolutions.StateMachines
{
	public class EventTransitionRequest
		: ATransitionRequest
	{
		private Type eventType;

		public Type EventType
		{
			get => eventType;
			set => eventType = value;
		}
	}
}