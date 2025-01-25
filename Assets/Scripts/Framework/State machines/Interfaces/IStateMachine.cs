using System;
using System.Collections.Generic;

namespace HereticalSolutions.StateMachines
{
    public interface IStateMachine<TBaseState>
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
        
        void Handle<TEvent>()
            where TEvent : ITransitionEvent<TBaseState>;
        
        void Handle(
            Type eventType);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        void TransitToImmediately<TState>()
            where TState : TBaseState;
        
        void TransitToImmediately(
            Type stateType);

        #endregion

        #region Scheduled transition

        IEnumerable<TransitionRequest<TBaseState>> ScheduledTransitions { get; }

        void ScheduleTransition<TState>(
            TransitionRequest<TBaseState> request);

        #endregion
    }
}