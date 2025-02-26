using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
	public class ConcurrentBaseStateMachine<TBaseState>
		: IStateMachine<TBaseState>
		where TBaseState : IState
	{
		private readonly IStateMachine<TBaseState> stateMachine;

		private readonly object lockObject;

		public ConcurrentBaseStateMachine(
			IStateMachine<TBaseState> stateMachine,
			object lockObject)
		{
			this.stateMachine = stateMachine;

			this.lockObject = lockObject;
		}

		#region IStateMachine

		public bool TransitionInProgress
		{
			get
			{
				lock (lockObject)
				{
					return stateMachine.TransitionInProgress;
				}
			}
		}

		#region Current state

		public TBaseState CurrentState
		{
			get
			{
				lock (lockObject)
				{
					return stateMachine.CurrentState;
				}
			}
		}

		public INonAllocSubscribable OnCurrentStateChangeStarted => stateMachine.OnCurrentStateChangeStarted;

		public INonAllocSubscribable OnCurrentStateChangeFinished => stateMachine.OnCurrentStateChangeFinished;

		#endregion

		#region All states

		public TConcreteState GetState<TConcreteState>()
			where TConcreteState : TBaseState
		{
			return stateMachine.GetState<TConcreteState>();
		}

		public TBaseState GetState(
			Type stateType)
		{
			return stateMachine.GetState(
				stateType);
		}

		public IEnumerable<Type> AllStates
		{
			get => stateMachine.AllStates;
		}

		#endregion

		#region Event handling

		public bool Handle<TEvent>(
			bool processQueueAfterFinish = true)
			where TEvent : ITransitionEvent<TBaseState>
		{
			lock (lockObject)
			{
				return stateMachine.Handle<TEvent>(
					processQueueAfterFinish);
			}
		}

		public bool Handle(
			Type eventType,
			bool processQueueAfterFinish = true)
		{
			lock (lockObject)
			{
				return stateMachine.Handle(
					eventType,
					processQueueAfterFinish);
			}
		}

		public INonAllocSubscribable OnEventFired => stateMachine.OnEventFired;

		#endregion

		#region Immediate transition

		public bool TransitToImmediately<TState>(
			bool processQueueAfterFinish = true)
			where TState : TBaseState
		{
			lock (lockObject)
			{
				return stateMachine.TransitToImmediately<TState>(
					processQueueAfterFinish);
			}
		}

		public bool TransitToImmediately(
			Type stateType,
			bool processQueueAfterFinish = true)
		{
			lock (lockObject)
			{
				return stateMachine.TransitToImmediately(
					stateType,
					processQueueAfterFinish);
			}
		}

		#endregion

		#region Scheduled transition

		public IEnumerable<ITransitionRequest> ScheduledTransitions => stateMachine.ScheduledTransitions;

		public void ScheduleTransition(
			ITransitionRequest request,
			bool startProcessingIfIdle = true)
		{
			lock (lockObject)
			{
				stateMachine.ScheduleTransition(
					request,
					startProcessingIfIdle);
			}
		}

		public void ProcessTransitionQueue()
		{
			lock (lockObject)
			{
				stateMachine.ProcessTransitionQueue();
			}
		}

		#endregion

		#endregion
	}
}