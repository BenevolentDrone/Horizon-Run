using System;

namespace HereticalSolutions.StateMachines
{
	public class EventAsyncTransitionRequest<TBaseState>
		: AAsyncTransitionRequest
		where TBaseState : IAsyncState
	{
		private IAsyncTransitionEvent<TBaseState> @event;
		public IAsyncTransitionEvent<TBaseState> Event
		{
			get
			{
				lock (lockObject)
				{
					return @event;
				}
			}
			set
			{
				lock (lockObject)
				{
					@event = value;
				}
			}
		}
	}
}