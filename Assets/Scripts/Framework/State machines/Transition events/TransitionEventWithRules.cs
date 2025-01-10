using HereticalSolutions.StateMachines;

namespace HereticalSolutions.StateMachines
{
    public class TransitionEventWithRules<TBaseState>
        : ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        public TBaseState From { get; protected set; }

        public TBaseState To { get; protected set; }

        public EAsyncTransitionRules Rules { get; protected set; }

        public TransitionEventWithRules(
            TBaseState from,
            TBaseState to,
            EAsyncTransitionRules rules)
        {
            From = from;
            To = to;
            Rules = rules;
        }

        public override string ToString()
        {
            return $"[{From.GetType().Name} => {To.GetType().Name}] (Rules: {Rules.ToString()})";
        }
    }
}