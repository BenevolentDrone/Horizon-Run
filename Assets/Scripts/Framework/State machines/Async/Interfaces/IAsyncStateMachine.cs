using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
    public interface IAsyncStateMachine<TBaseState>
        where TBaseState : IAsyncState
    {
        bool TransitionInProgress { get; }
        
        #region Current state

        TBaseState CurrentState { get; }
        
        INonAllocSubscribable OnCurrentStateChangeStarted { get; }

        INonAllocSubscribable OnCurrentStateChangeFinished { get; }

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
        
        INonAllocSubscribable OnEventFired { get; }
        
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

        IEnumerable<IAsyncTransitionRequest> ScheduledTransitions { get; }

        void ScheduleTransition(
            IAsyncTransitionRequest request);

        #endregion
    }
}