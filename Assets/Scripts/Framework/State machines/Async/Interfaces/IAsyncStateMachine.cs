using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

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
            AsyncExecutionContext asyncContext);
        
        Task Handle(
            Type eventType,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            AsyncExecutionContext asyncContext);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            AsyncExecutionContext asyncContext);
        
        Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            AsyncExecutionContext asyncContext);

        #endregion

        #region Scheduled transition

        void ScheduleTransition<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            AsyncExecutionContext asyncContext);

        void ScheduleTransition(
            Type stateType,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null,

            //Async tail
            AsyncExecutionContext asyncContext);

        #endregion
    }
}