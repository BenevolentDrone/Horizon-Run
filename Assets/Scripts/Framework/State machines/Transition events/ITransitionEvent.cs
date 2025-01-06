namespace HereticalSolutions.StateMachines
{
    public interface ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        TBaseState From { get; }
        
        TBaseState To { get; }
    }
}