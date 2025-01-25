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
			private readonly IReadOnlyRepository<Type, TBaseState> states;

			private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

			private readonly ITransitionController<TBaseState> transitionController;

			private readonly Queue<ITransitionRequest<TBaseState>> transitionQueue;


			private readonly INonAllocSubscribable onCurrentStateChangeStarted;
	
			private readonly INonAllocSubscribable onCurrentStateChangeFinished;


			private readonly ILogger logger;


			private TBaseState currentState;

			private bool transitionInProgress;

			public BaseStateMachine(
				IReadOnlyRepository<Type, TBaseState> states,
				IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
				Queue<ITransitionEvent<TBaseState>> transitionQueue,

				INonAllocSubscribable onCurrentStateChangeStarted,
				INonAllocSubscribable onCurrentStateChangeFinished,

				TBaseState initialState,

				ILogger logger)
			{
				this.states = states;

				this.events = events;

				this.transitionQueue = transitionQueue;

				this.logger = logger;


				this.onCurrentStateChangeStarted = onCurrentStateChangeStarted;
				
				this.onCurrentStateChangeFinished = onCurrentStateChangeFinished;


				currentState = initialState;
			}

			#region IStateMachine

			public bool TransitionInProgress => transitionInProgress;

			#region Current state

			public TBaseState CurrentState => currentState;

			public INonAllocSubscribable OnCurrentStateChangeStarted => onCurrentStateChangeStarted;

			public INonAllocSubscribable OnCurrentStateChangeFinished => onCurrentStateChangeFinished;

			//public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }
	
			//public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }
	
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
	
			public bool Handle<TEvent>()
			{
				if (TransitionInProgress
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

				if (TransitionInProgress)
					transitionQueue.Enqueue(@event);
				else
					PerformTransition(@event);
			}

			public bool Handle(
				Type eventType)
			{
				ITransitionEvent<TBaseState> @event;

				if (!events.TryGet(eventType, out @event))
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"EVENT {eventType.Name} NOT FOUND"));

				if (TransitionInProgress)
					transitionQueue.Enqueue(@event);
				else
					PerformTransition(@event);
			}

			public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }

			#endregion

			#region Immediate transition

			public void TransitToImmediately<TState>()
			{
				if (!states.Has(typeof(TState)))
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"STATE {nameof(TState)} NOT FOUND"));

				var previousState = CurrentState;
				var newState = states.Get(typeof(TState));

				PerformTransition(
					previousState,
					newState);
			}

			public void TransitToImmediately(Type stateType)
			{
				if (!states.Has(stateType))
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"STATE {stateType.Name} NOT FOUND"));

				var previousState = CurrentState;
				var newState = states.Get(stateType);

				PerformTransition(
					previousState,
					newState);
			}

			#endregion

			#endregion

			private void PerformTransition(ITransitionEvent<TBaseState> @event)
			{
				if (!EqualityComparer<TBaseState>.Default.Equals(CurrentState, @event.From))
				{
					string currentStateString = CurrentState.GetType().Name;
					string fromStateString = @event.From.GetType().Name;

					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION FROM STATE {fromStateString}"));
				}

				OnEventFired?.Invoke(
					@event);

				var previousState = CurrentState;

				var newState = @event.To;

				PerformTransition(
					previousState,
					newState);
			}

			private void PerformTransition(
				TBaseState previousState,
				TBaseState newState)
			{
				TransitionInProgress = true;

				OnCurrentStateChangeStarted?.Invoke(
					previousState,
					newState);

				previousState.ExitState();

				currentState = newState;

				newState.EnterState();

				OnCurrentStateChangeFinished?.Invoke(previousState, newState);

				TransitionInProgress = false;

				if (transitionQueue.Count != 0)
				{
					PerformTransition(transitionQueue.Dequeue());
				}
			}
		}
	}