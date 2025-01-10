using System;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StateMachines
{
    public interface IAsyncStateMachine<TBaseState>
        where TBaseState : IState
    {
        bool TransitionInProgress { get; }
        
        #region Current state

        TBaseState CurrentState { get; }
        
        Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }
        
        Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        TBaseState GetState<TConcreteState>();
        
        TBaseState GetState(
            Type stateType);
        
        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
        
        Task Handle(
            Type eventType,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
        
        Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        #endregion

        #region Scheduled transition

        void ScheduleTransition<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        void ScheduleTransition(
            Type stateType,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        #endregion
    }
}