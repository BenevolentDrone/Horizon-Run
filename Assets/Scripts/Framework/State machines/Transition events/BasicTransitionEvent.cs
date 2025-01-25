namespace HereticalSolutions.StateMachines
{
    public class BasicTransitionEvent<TBaseState>
        : ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        public TBaseState From { get; protected set; }
        
        public TBaseState To { get; protected set; }
        
        public BasicTransitionEvent(
            TBaseState from,
            TBaseState to)
        {
            From = from;
            
            To = to;
        }
        
        public override string ToString()
        {
            return $"[{From.GetType().Name} => {To.GetType().Name}]";
        }
    }
}