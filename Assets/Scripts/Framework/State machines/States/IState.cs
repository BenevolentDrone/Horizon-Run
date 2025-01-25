namespace HereticalSolutions.StateMachines
{
    public interface IState
    {
        void EnterState();

        void EnterState(
            ITransitionRequest transitionRequest);
        
        void ExitState();

        void ExitState(
            ITransitionRequest transitionRequest);
    }
}