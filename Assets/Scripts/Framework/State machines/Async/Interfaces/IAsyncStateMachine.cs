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
            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);
        
        Task Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        Task TransitToImmediately<TState>(
            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);
        
        Task TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);

        #endregion

        #region Scheduled transition

        void ScheduleTransition<TState>(
            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);

        void ScheduleTransition(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null,
            TransitionSupervisor supervisor = null);

        #endregion
    }
}