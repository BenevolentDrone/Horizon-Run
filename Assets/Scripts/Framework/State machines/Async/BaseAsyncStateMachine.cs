using System;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Logging;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines
{
    public class BaseAsyncStateMachine<TBaseState>
        : IAsyncStateMachine<TBaseState>
        where TBaseState : IAsyncState
    {
        private readonly IReadOnlyRepository<Type, TBaseState> states;

        private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

        private readonly Queue<AsyncTransitionRequest<TBaseState>> transitionRequestsQueue;

        private readonly IAsyncTransitionController<TBaseState> transitionController;

        private readonly EAsyncTransitionRules defaultAsyncTransitionRules;

        private readonly object lockObject;

        private readonly ILogger logger;


        private Task processTransitionQueueTask;

        private CancellationTokenSource processTransitionQueueCancellationTokenSource;

        private CancellationTokenSource transitToImmediatelyCancellationTokenSource;




        private bool transitionInProgress;

        private TBaseState currentState;




        public BaseAsyncStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            Queue<AsyncTransitionRequest<TBaseState>> transitionRequestsQueue,
            IAsyncTransitionController<TBaseState> transitionController,
            EAsyncTransitionRules defaultAsyncTransitionRules,
            TBaseState currentState,
            ILogger logger)
        {
            this.states = states;

            this.events = events;

            this.transitionRequestsQueue = transitionRequestsQueue;

            this.transitionController = transitionController;

            this.defaultAsyncTransitionRules = defaultAsyncTransitionRules;

            this.logger = logger;


            lockObject = new object();


            CurrentState = currentState;

            OnCurrentStateChangeStarted = null;

            OnCurrentStateChangeFinished = null;

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
        //TODO: FINISH THE REST OF THE CLASS REFACTORING

        public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }

        public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        public TBaseState GetState<TConcreteState>()
        {
            if (!states.TryGet(typeof(TConcreteState), out var result))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {nameof(TConcreteState)} NOT FOUND"));

            return result;
        }

        /// <summary>
        /// Gets the state of the specified type
        /// </summary>
        /// <param name="stateType">The type of the state.</param>
        /// <returns>The state instance.</returns>
        public TBaseState GetState(Type stateType)
        {
            if (!states.TryGet(stateType, out var result))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            return result;
        }

        /// <summary>
        /// Gets all the types of states in the state machine
        /// </summary>
        public IEnumerable<Type> AllStates => states.Keys;

        #endregion

        #region Event handling

        /// <summary>
        /// Handles the specified event asynchronously
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {nameof(TEvent)} NOT FOUND"));

            var request = new TransitionRequest<TBaseState>(
                @event,
                new CancellationTokenSource(),
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != ETransitionState.ABORTED
                   && request.TransitionState != ETransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event type asynchronously
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(
            Type eventType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {eventType.Name} NOT FOUND"));

            var request = new TransitionRequest<TBaseState>(
                @event,
                new CancellationTokenSource(),
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != ETransitionState.ABORTED
                   && request.TransitionState != ETransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event asynchronously with a cancellation token
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle<TEvent>(
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {nameof(TEvent)} NOT FOUND"));

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var request = new TransitionRequest<TBaseState>(
                @event,
                cancellationTokenSource,
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != ETransitionState.ABORTED
                   && request.TransitionState != ETransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Handles the specified event type asynchronously with a cancellation token
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(
            Type eventType,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {eventType.Name} NOT FOUND"));

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var request = new TransitionRequest<TBaseState>(
                @event,
                cancellationTokenSource,
                stateExitProgress,
                stateEnterProgress,
                protocol);

            lock (lockObject)
            {
                transitionRequestsQueue.Enqueue(request);

                if (processTransitionQueueTask == null
                    || processTransitionQueueTask.IsCompleted)
                {
                    processTransitionQueueCancellationTokenSource?.Dispose();

                    processTransitionQueueCancellationTokenSource = new CancellationTokenSource();

                    processTransitionQueueTask = ProcessTransitionQueueTask(processTransitionQueueCancellationTokenSource.Token);
                }
            }

            while (request.TransitionState != ETransitionState.ABORTED
                   && request.TransitionState != ETransitionState.COMPLETED)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Gets or sets the action invoked when an event is fired
        /// </summary>
        public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }

        #endregion

        #region Immediate transition

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {nameof(TState)} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    var task = PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol);

                    await task;
                        //.ConfigureAwait(false);

                    await task
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (!states.Has(stateType))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    var task = PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol);

                    await task;
                        //.ConfigureAwait(false);

                    await task
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously with a cancellation token
        /// </summary>
        /// <typeparam name="TState">The type of the state to transition to.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately<TState>(
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {nameof(TState)} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    var task = PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol);

                    await task;
                        //.ConfigureAwait(false);

                    await task
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Transitions immediately to the specified state type asynchronously with a cancellation token
        /// </summary>
        /// <param name="stateType">The type of the state to transition to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="stateExitProgress">The progress reporter for the state exit.</param>
        /// <param name="stateEnterProgress">The progress reporter for the state enter.</param>
        /// <param name="protocol">The transition protocol.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TransitToImmediately(
            Type stateType,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (!states.Has(stateType))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            ClearTransitionQueue();

            CancelImmediateTransitions();

            using (transitToImmediatelyCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    var task = PerformTransition(
                        previousState,
                        newState,
                        defaultAsyncTransitionRules,
                        transitToImmediatelyCancellationTokenSource.Token,
                        stateExitProgress,
                        stateEnterProgress,
                        protocol);

                    await task;
                        //.ConfigureAwait(false);

                    await task
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        #endregion

        #endregion

        private async Task ProcessTransitionQueueTask(
            CancellationToken cancellationToken)
        {
            transitionInProgress = true;

            while (
                transitionRequestsQueue.Count > 0
                && !cancellationToken.IsCancellationRequested)
            {
                AsyncTransitionRequest<TBaseState> nextRequest;

                lock (lockObject) 
                {
                    nextRequest = transitionRequestsQueue.Dequeue();
                }

                nextRequest.TransitionState = ETransitionState.IN_PROGRESS;

                using (CancellationTokenSource combinedTokenSource =
                       CancellationTokenSource.CreateLinkedTokenSource(
                           nextRequest.CancellationTokenSource.Token,
                           cancellationToken))
                {
                    try
                    {
                        var task = PerformTransition(
                            nextRequest.Event,
                            nextRequest.CancellationTokenSource.Token,
                            nextRequest.StateExitProgress,
                            nextRequest.StateEnterProgress,
                            nextRequest.TransitionController);

                        await task;
                            //.ConfigureAwait(false);

                        await task
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);
                    }
                    catch (Exception e)
                    {
                        nextRequest.TransitionState = ETransitionState.ABORTED;
                    }
                    finally
                    {
                        if (combinedTokenSource.Token.IsCancellationRequested)
                        {
                            nextRequest.TransitionState = ETransitionState.ABORTED;
                        }
                        else
                        {
                            nextRequest.TransitionState = ETransitionState.COMPLETED;
                        }

                        combinedTokenSource?.Dispose();
                    }
                }
                
                nextRequest.CancellationTokenSource?.Dispose();
            }

            transitionInProgress = false;
        }
        
        private void ClearTransitionQueue()
        {
            processTransitionQueueCancellationTokenSource?.Dispose();
            
            processTransitionQueueCancellationTokenSource?.Cancel();
            
            lock (lockObject)
            {
                foreach (var request in transitionRequestsQueue)
                {
                    request.TransitionState = ETransitionState.ABORTED;
                }

                transitionRequestsQueue.Clear();
            }
        }

        private void CancelImmediateTransitions()
        {
            transitToImmediatelyCancellationTokenSource?.Dispose();
            
            transitToImmediatelyCancellationTokenSource?.Cancel();
        }
        
        private async Task PerformTransition(
            ITransitionEvent<TBaseState> @event,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
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

            OnEventFired?.Invoke(@event);

            var previousState = CurrentState;

            var newState = @event.To;

            var rules = defaultAsyncTransitionRules;

            if (@event is AsyncTransitionEvent<TBaseState>)
                rules = ((AsyncTransitionEvent<TBaseState>)@event).Rules;

            var task = PerformTransition(
                previousState,
                newState,
                rules,
                cancellationToken,
                stateExitProgress,
                stateEnterProgress,
                protocol);

            await task;
                //.ConfigureAwait(false);

            await task
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);
        }

        private async Task PerformTransition(
            TBaseState previousState,
            TBaseState newState,
            EAsyncTransitionRules rules,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            try
            {
                switch (rules)
                {
                    case EAsyncTransitionRules.EXIT_THEN_ENTER:

                        OnCurrentStateChangeStarted?.Invoke(
                            previousState,
                            newState);

                        var exitStateTask1 = ExitState(
                            previousState,
                            cancellationToken,
                            stateExitProgress,
                            protocol);

                        await exitStateTask1;
                            //.ConfigureAwait(false);

                        await exitStateTask1
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        CurrentState = newState;

                        var enterStateTask1 = EnterState(
                            newState,
                            cancellationToken,
                            stateEnterProgress,
                            protocol);

                        await enterStateTask1;
                            //.ConfigureAwait(false);

                        await enterStateTask1
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;

                    case EAsyncTransitionRules.ENTER_THEN_EXIT:
                        
                        OnCurrentStateChangeStarted?.Invoke(
                            previousState,
                            newState);

                        var enterStateTask2 = EnterState(
                            newState,
                            cancellationToken,
                            stateEnterProgress,
                            protocol);

                        await enterStateTask2;
                            //.ConfigureAwait(false);

                        await enterStateTask2
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        CurrentState = newState;

                        var exitStateTask2 = ExitState(
                            previousState,
                            cancellationToken,
                            stateExitProgress,
                            protocol);

                        await exitStateTask2;
                            //.ConfigureAwait(false);

                        await exitStateTask2
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;

                    case EAsyncTransitionRules.CONCURRENT:

                        OnCurrentStateChangeStarted?.Invoke(previousState, newState);

                        var enterExitStateTask = Task
                            .WhenAll(
                                EnterState(
                                    newState,
                                    cancellationToken,
                                    stateEnterProgress,
                                    protocol),
                                ExitState(
                                    previousState,
                                    cancellationToken,
                                    stateExitProgress,
                                    protocol));

                        await enterExitStateTask;
                            //.ConfigureAwait(false);

                        await enterExitStateTask
                            .ThrowExceptionsIfAny(
                                GetType(),
                                logger);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        CurrentState = newState;

                        OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                        break;
                }
            }
            catch (OperationCanceledException)
            {
                //BOING
            }
        }

        private async Task ExitState(
            TBaseState previousState,
            CancellationToken cancellationToken,
            IProgress<float> stateExitProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (protocol != null)
            {
                while (protocol.CommencePreviousStateExitStart != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            var task = transitionController.ExitState(
                previousState,
                cancellationToken,
                stateExitProgress);

            await task;
                //.ConfigureAwait(false);

            await task
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
                        
            previousState.ExitState();

            protocol?.OnPreviousStateExited?.Invoke(previousState);
                        
            if (protocol != null)
            {
                while (protocol.CommencePreviousStateExitFinish != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private async Task EnterState(
            TBaseState newState,
            CancellationToken cancellationToken,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor protocol = null)
        {
            if (protocol != null)
            {
                while (protocol.CommenceNextStateEnterStart != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
                        
            var task = transitionController.EnterState(
                newState,
                cancellationToken,
                stateEnterProgress);

            await task;
                //.ConfigureAwait(false);

            await task
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            newState.EnterState();
                        
            protocol?.OnNextStateEntered?.Invoke(newState);
                        
            if (protocol != null)
            {
                while (protocol.CommenceNextStateEnterFinish != true)
                {
                    await Task.Yield();
                                
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }
    }
}