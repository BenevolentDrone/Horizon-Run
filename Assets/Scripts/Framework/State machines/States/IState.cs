namespace HereticalSolutions.StateMachines
{
    public interface IState
    {
        void OnStateEntered();
        
        void OnStateExited();
    }
}