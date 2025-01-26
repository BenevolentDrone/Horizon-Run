using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
    public interface IStateMachine<TBaseState>
        where TBaseState : IState
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
        
        bool Handle<TEvent>(
            bool processQueueAfterFinish = true)
            where TEvent : ITransitionEvent<TBaseState>;

        bool Handle(
            Type eventType,
            bool processQueueAfterFinish = true);

        INonAllocSubscribable OnEventFired { get; }

        #endregion

        #region Immediate transition

        bool TransitToImmediately<TState>(
            bool processQueueAfterFinish = true)
            where TState : TBaseState;
        
        bool TransitToImmediately(
            Type stateType,
            bool processQueueAfterFinish = true);

        #endregion

        #region Scheduled transition

        IEnumerable<ITransitionRequest> ScheduledTransitions { get; }

        void ScheduleTransition(
            ITransitionRequest request,
            bool startProcessingIfIdle = true);

        void ProcessTransitionQueue();

        #endregion
    }
}