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

        TBaseState GetState<TConcreteState>();

        TBaseState GetState(Type stateType);

        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        void Handle<TEvent>();
        
        void Handle(Type eventType);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        void TransitToImmediately<TState>();
        
        void TransitToImmediately(Type stateType);
        
        #endregion
    }
}