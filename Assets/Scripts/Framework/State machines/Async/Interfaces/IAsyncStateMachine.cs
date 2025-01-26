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
        
        Task<bool> Handle<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
            where TEvent : IAsyncTransitionEvent<TBaseState>;

        Task<bool> Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true);
        
        INonAllocSubscribable OnEventFired { get; }
        
        #endregion

        #region Immediate transition
        
        Task<bool> TransitToImmediately<TState>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true)
            where TState : TBaseState;
        
        Task<bool> TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool processQueueAfterFinish = true);

        #endregion

        #region Scheduled transition

        IEnumerable<IAsyncTransitionRequest> ScheduledTransitions { get; }

        Task ScheduleTransition(
            IAsyncTransitionRequest request,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool startProcessingIfIdle = true);

        Task ProcessTransitionQueue(
            
            //Async tail
            AsyncExecutionContext asyncContext);

        #endregion
    }
}