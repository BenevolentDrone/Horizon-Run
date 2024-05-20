namespace HereticalSolutions.StateMachines
{
    public interface IStateWithAnimatedTransition : IState
    {
        AnimationHandler StateEnterAnimationHandler { get; }
        
        AnimationHandler StateExitAnimationHandler { get; }
    }
}