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

        TConcreteState GetState<TConcreteState>()
            where TConcreteState : TBaseState;
        
        TBaseState GetState(
            Type stateType);
        
        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        Task Handle<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext);
        
        Task Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        Task TransitToImmediately<TState>(

            //Async tail
            AsyncExecutionContext asyncContext)
            where TState : TBaseState;
        
        Task TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext);

        #endregion

        #region Scheduled transition

        IEnumerable<AsyncTransitionRequest<TBaseState>> ScheduledTransitions { get; }

        void ScheduleTransition<TState>(
            AsyncTransitionRequest<TBaseState> request);

        #endregion
    }
}