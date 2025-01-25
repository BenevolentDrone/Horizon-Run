namespace HereticalSolutions.StateMachines
{
    public interface IState
    {
        void EnterState();
        
        void ExitState();
    }
}