using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StateMachines
{
	public class BaseStateMachine<TBaseState>
		: IStateMachine<TBaseState>
		where TBaseState : IState
	{
		private static readonly EqualityComparer<TBaseState> comparer = EqualityComparer<TBaseState>.Default;

		private readonly IReadOnlyRepository<Type, TBaseState> states;

		private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

		private readonly ITransitionController<TBaseState> transitionController;

		private readonly Queue<ITransitionRequest> transitionQueue;


		private readonly INonAllocSubscribable onCurrentStateChangeStarted;

		private readonly INonAllocSubscribable onCurrentStateChangeFinished;

		private readonly INonAllocSubscribable onEventFired;


		private readonly ILogger logger;


		private TBaseState currentState;

		private bool transitionInProgress;

		public BaseStateMachine(
			IReadOnlyRepository<Type, TBaseState> states,
			IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,

			ITransitionController<TBaseState> transitionController,
			Queue<ITransitionRequest> transitionQueue,

			INonAllocSubscribable onCurrentStateChangeStarted,
			INonAllocSubscribable onCurrentStateChangeFinished,
			INonAllocSubscribable onEventFired,

			TBaseState initialState,

			ILogger logger)
		{
			this.states = states;

			this.events = events;


			this.transitionController = transitionController;

			this.transitionQueue = transitionQueue;


			this.onCurrentStateChangeStarted = onCurrentStateChangeStarted;
			
			this.onCurrentStateChangeFinished = onCurrentStateChangeFinished;

			this.onEventFired = onEventFired;


			this.logger = logger;


			currentState = initialState;

			transitionInProgress = false;
		}

		#region IStateMachine

		public bool TransitionInProgress => transitionInProgress;

		#region Current state

		public TBaseState CurrentState => currentState;

		public INonAllocSubscribable OnCurrentStateChangeStarted => onCurrentStateChangeStarted;

		public INonAllocSubscribable OnCurrentStateChangeFinished => onCurrentStateChangeFinished;

		#endregion

		#region All states

		public TConcreteState GetState<TConcreteState>()
			where TConcreteState : TBaseState
		{
			if (!states.TryGet(
				typeof(TConcreteState),
				out var result))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {nameof(TConcreteState)} NOT FOUND"));
			}

			return (TConcreteState)result;
		}

		public TBaseState GetState(
			Type stateType)
		{
			if (!states.TryGet(
				stateType,
				out var result))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {stateType.Name} NOT FOUND"));
			}

			return result;
		}

		public IEnumerable<Type> AllStates
		{
			get => states.Keys;
		}

		#endregion

		#region Event handling

		public bool Handle<TEvent>(
			bool processQueueAfterFinish = true)
			where TEvent : ITransitionEvent<TBaseState>
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				return false;
			}

			ITransitionEvent<TBaseState> @event;

			if (!events.TryGet(
				typeof(TEvent),
				out @event))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"EVENT {nameof(TEvent)} NOT FOUND"));
			}

			PerformTransition(
				@event,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public bool Handle(
			Type eventType,
			bool processQueueAfterFinish = true)
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				return false;
			}

			ITransitionEvent<TBaseState> @event;

			if (!events.TryGet(
				eventType,
				out @event))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"EVENT {eventType.Name} NOT FOUND"));
			}

			PerformTransition(
				@event,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public INonAllocSubscribable OnEventFired => onEventFired;

		#endregion

		#region Immediate transition

		public bool TransitToImmediately<TState>(
			bool processQueueAfterFinish = true)
			where TState : TBaseState
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				return false;
			}

			if (!states.TryGet(
				typeof(TState),
				out var newState))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {nameof(TState)} NOT FOUND"));
			}

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public bool TransitToImmediately(
			Type stateType,
			bool processQueueAfterFinish = true)
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				return false;
			}

			if (!states.TryGet(
				stateType,
				out var newState))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {stateType.Name} NOT FOUND"));

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		#endregion
	
		#region Scheduled transition
	
		public IEnumerable<ITransitionRequest> ScheduledTransitions => transitionQueue;
	
		public void ScheduleTransition(
			ITransitionRequest request,
			bool startProcessingIfIdle = true)
		{
			if (request.TransitionState != ETransitionState.UNINITIALISED)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"TRANSITION REQUEST {request.GetType().Name} ALREADY SCHEDULED"));
			}

			transitionQueue.Enqueue(request);

			request.TransitionState = ETransitionState.QUEUED;

			if (startProcessingIfIdle
				&& !transitionInProgress)
			{
				ProcessTransitionQueue();
			}
		}

		public void ProcessTransitionQueue()
		{
			if (transitionInProgress)
			{
				return;
			}

			if (transitionQueue.Count == 0)
			{
				return;
			}

			while (transitionQueue.Count != 0)
			{
				var transitionRequest = transitionQueue.Dequeue();

				switch (transitionRequest)
				{
					case EventTransitionRequest eventTransitionRequest:
					{
						ITransitionEvent<TBaseState> @event;

						if (!events.TryGet(
							eventTransitionRequest.EventType,
							out @event))
						{
							throw new Exception(
								logger.TryFormatException(
									GetType(),
									$"EVENT {eventTransitionRequest.EventType.Name} NOT FOUND"));
						}

						PerformTransition(
							@event,
							transitionRequest);

						break;
					}

					case ImmediateTransitionRequest immediateTransitionRequest:
					{
						if (!states.TryGet(
							immediateTransitionRequest.TargetStateType,
							out var newState))
						{
							throw new Exception(
								logger.TryFormatException(
									GetType(),
									$"STATE {immediateTransitionRequest.TargetStateType.Name} NOT FOUND"));
						}

						var previousState = currentState;

						PerformTransition(
							previousState,
							newState,
							transitionRequest);

						break;
					}
				}
			}
		}

		#endregion

		#endregion

		private void PerformTransition(
			ITransitionEvent<TBaseState> @event,
			ITransitionRequest transitionRequest)
		{
			if (!comparer.Equals(
				currentState,
				@event.From))
			{
				string currentStateString = currentState.GetType().Name;

				string transitionString = @event.GetType().Name;

				string fromStateString = @event.From.GetType().Name;

				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION {transitionString} PREVIOUS STATE {fromStateString}"));
			}

			var publisher = onEventFired as IPublisherSingleArgGeneric<ITransitionEvent<TBaseState>>;

			publisher?.Publish(
				@event);

			var previousState = currentState;

			var newState = @event.To;

			PerformTransition(
				previousState,
				newState,
				transitionRequest);
		}

		private void PerformTransition(
			TBaseState previousState,
			TBaseState newState,
			ITransitionRequest transitionRequest)
		{
			transitionInProgress = true;

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.IN_PROGRESS;
			}

			#region Exit previous state

			object[] args = new object[]
			{
				previousState,
				newState
			};

			var stateChangeStartPublisher = onCurrentStateChangeStarted
				as IPublisherMultipleArgs;

			stateChangeStartPublisher?.Publish(
				args);

			if (transitionRequest != null)
				transitionController.ExitState(
					previousState,
					transitionRequest);
			else
				transitionController.ExitState(
					previousState);

			#endregion

			currentState = newState;

			#region Enter new state

			if (transitionRequest != null)
				transitionController.EnterState(
					previousState,
					transitionRequest);
			else
				transitionController.EnterState(
					previousState);

			var stateChangeFinishPublisher = onCurrentStateChangeFinished
				as IPublisherMultipleArgs;

			stateChangeFinishPublisher?.Publish(
				args);

			#endregion

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.COMPLETED;
			}

			transitionInProgress = false;
		}
	}
}