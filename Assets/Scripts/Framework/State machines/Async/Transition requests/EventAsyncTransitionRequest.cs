using System;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
	public class EventAsyncTransitionRequest<TBaseState>
		: AsyncTransitionRequest<TBaseState>
		where TBaseState : IState
	{
		private ITransitionEvent<TBaseState> @event;
		public ITransitionEvent<TBaseState> Event
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