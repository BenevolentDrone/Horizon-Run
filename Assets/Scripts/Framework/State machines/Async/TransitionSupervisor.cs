using System;

namespace HereticalSolutions.StateMachines
{
    public class TransitionSupervisor
    {
        public bool CommencePreviousStateExitStart { get; set; }

        public Action<IState> OnPreviousStateExited { get; set; }
        
        public bool CommencePreviousStateExitFinish { get; set; }

        
        public bool CommenceNextStateEnterStart { get; set; }

        public Action<IState> OnNextStateEntered { get; set; }
        
        public bool CommenceNextStateEnterFinish { get; set; }


        public TransitionSupervisor()
        {
            CommencePreviousStateExitStart = true;

            CommencePreviousStateExitFinish = true;
            
        
            CommenceNextStateEnterStart = true;

            CommenceNextStateEnterFinish = true;
        }
    }
}