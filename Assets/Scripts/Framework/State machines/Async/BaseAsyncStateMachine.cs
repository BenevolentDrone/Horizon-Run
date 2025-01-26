using System;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    public class BaseAsyncStateMachine<TBaseState>
        : IAsyncStateMachine<TBaseState>
        where TBaseState : IAsyncState
    {
        private static readonly EqualityComparer<TBaseState> comparer = EqualityComparer<TBaseState>.Default;

        private readonly IReadOnlyRepository<Type, TBaseState> states;

        private readonly IReadOnlyRepository<Type, IAsyncTransitionEvent<TBaseState>> events;

        private readonly IAsyncTransitionController<TBaseState> transitionController;

        private readonly Queue<IAsyncTransitionRequest> transitionQueue;


        private readonly INonAllocSubscribable onCurrentStateChangeStarted;

        private readonly INonAllocSubscribable onCurrentStateChangeFinished;

        private readonly INonAllocSubscribable onEventFired;


        private readonly object lockObject;


        private readonly ILogger logger;


        private TBaseState currentState;

        private bool transitionInProgress;

        public BaseAsyncStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, IAsyncTransitionEvent<TBaseState>> events,

            IAsyncTransitionController<TBaseState> transitionController,
            Queue<IAsyncTransitionRequest> transitionQueue,

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


            lockObject = new object();


            this.logger = logger;


            currentState = initialState;

            transitionInProgress = false;
        }

        #region IAsyncStateMachine

        public bool TransitionInProgress
        {
            get
            {
                lock (lockObject)
                {
                    return transitionInProgress;
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
                    return currentState;
                }
            }
        }

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

        public async Task<bool> Handle<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
            where TEvent : IAsyncTransitionEvent<TBaseState>
        {
            IAsyncTransitionEvent<TBaseState> @event;

            lock (lockObject)
            {
                if (transitionInProgress
                    || transitionQueue.Count != 0)
                {
                    return false;
                }

                if (!events.TryGet(
                    typeof(TEvent),
                    out @event))
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"EVENT {nameof(TEvent)} NOT FOUND"));
                }
            }

            await PerformTransition(
                @event,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public async Task<bool> Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
        {
            IAsyncTransitionEvent<TBaseState> @event;

            lock (lockObject)
            {
                if (transitionInProgress
                    || transitionQueue.Count != 0)
                {
                    return false;
                }

                if (!events.TryGet(
                    eventType,
                    out @event))
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"EVENT {eventType.Name} NOT FOUND"));
                }
            }

            await PerformTransition(
                @event,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public INonAllocSubscribable OnEventFired => onEventFired;

        #endregion

        #region Immediate transition

        public async Task<bool> TransitToImmediately<TState>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
            where TState : TBaseState
        {
            TBaseState previousState;

            TBaseState newState;

            lock (lockObject)
            {
                if (transitionInProgress
                    || transitionQueue.Count != 0)
                {
                    return false;
                }

                if (!states.TryGet(
                    typeof(TState),
                    out newState))
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"STATE {nameof(TState)} NOT FOUND"));
                }

                previousState = currentState;
            }
            
            await PerformTransition(
                previousState,
                newState,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public async Task<bool> TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
        {
            TBaseState previousState;

            TBaseState newState;

            lock (lockObject)
            {
                if (transitionInProgress
                    || transitionQueue.Count != 0)
                {
                    return false;
                }

                if (!states.TryGet(
                    stateType,
                    out newState))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"STATE {stateType.Name} NOT FOUND"));

                previousState = currentState;
            }

            await PerformTransition(
                previousState,
                newState,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        #endregion

        #region Scheduled transition

        public IEnumerable<IAsyncTransitionRequest> ScheduledTransitions => transitionQueue;

        public async Task ScheduleTransition(
            IAsyncTransitionRequest request,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool startProcessingIfIdle = true)
        {
            bool startProcessing;

            lock (lockObject)
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

                startProcessing =
                    startProcessingIfIdle
                    && !transitionInProgress;
            }

            if (startProcessing)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }
        }

        public async Task ProcessTransitionQueue(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            lock (lockObject)
            {
                if (transitionInProgress)
                {
                    return;
                }

                if (transitionQueue.Count == 0)
                {
                    return;
                }
            }

            bool queueEmpty = false;

            while (!queueEmpty) //(transitionQueue.Count != 0)
            {
                IAsyncTransitionRequest transitionRequest;

                lock (lockObject)
                {
                    transitionRequest = transitionQueue.Dequeue();
                }
                
                switch (transitionRequest)
                {
                    case EventTransitionRequest eventTransitionRequest:
                    {
                        IAsyncTransitionEvent<TBaseState> @event;

                        lock (lockObject)
                        {
                            if (!events.TryGet(
                                eventTransitionRequest.EventType,
                                out @event))
                            {
                                throw new Exception(
                                    logger.TryFormatException(
                                        GetType(),
                                        $"EVENT {eventTransitionRequest.EventType.Name} NOT FOUND"));
                            }
                        }

                        await PerformTransition(
                            @event,
                            transitionRequest,
                            
                            asyncContext);

                        break;
                    }

                    case ImmediateTransitionRequest immediateTransitionRequest:
                    {
                        TBaseState previousState;

                        TBaseState newState;

                        lock (lockObject)
                        {
                            if (!states.TryGet(
                                immediateTransitionRequest.TargetStateType,
                                out newState))
                            {
                                throw new Exception(
                                    logger.TryFormatException(
                                        GetType(),
                                        $"STATE {immediateTransitionRequest.TargetStateType.Name} NOT FOUND"));
                            }
    
                            previousState = currentState;
                        }

                        await PerformTransition(
                            previousState,
                            newState,
                            transitionRequest,
                            
                            asyncContext);

                        break;
                    }
                }

                lock (lockObject)
                {
                    queueEmpty = transitionQueue.Count == 0;
                }
            }
        }

        #endregion

        #endregion

        private async Task PerformTransition(
            IAsyncTransitionEvent<TBaseState> @event,
            IAsyncTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            TBaseState previousState;

            TBaseState newState;

            lock (lockObject)
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

                var publisher = onEventFired as IPublisherSingleArgGeneric<IAsyncTransitionEvent<TBaseState>>;
    
                publisher?.Publish(
                    @event);
    
                previousState = currentState;
    
                newState = @event.To;
            }

            await PerformTransition(
                previousState,
                newState,
                transitionRequest,

                asyncContext);
        }

        private async Task PerformTransition(
            TBaseState previousState,
            TBaseState newState,
            IAsyncTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            object[] args;

            lock (lockObject)
            {
                transitionInProgress = true;

                if (transitionRequest != null)
                {
                    transitionRequest.TransitionState = ETransitionState.IN_PROGRESS;
                }
    
                #region Exit previous state
    
                args = new object[]
                {
                    previousState,
                    newState
                };
    
                var stateChangeStartPublisher = onCurrentStateChangeStarted
                    as IPublisherMultipleArgs;
    
                stateChangeStartPublisher?.Publish(
                    args);
            }

            if (transitionRequest != null)
                await transitionController.ExitState(
                    previousState,
                    transitionRequest,

                    asyncContext);
            else
                await transitionController.ExitState(
                    previousState,
                    
                    asyncContext);

            #endregion

            lock (lockObject)
            {
                currentState = newState;
            }

            #region Enter new state

            if (transitionRequest != null)
                await transitionController.EnterState(
                    previousState,
                    transitionRequest,
                    
                    asyncContext);
            else
                await transitionController.EnterState(
                    previousState,
                    
                    asyncContext);

            lock (lockObject)
            {

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
}