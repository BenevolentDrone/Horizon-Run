using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    public class AnimationTransitionController<TBaseState> : ITransitionController<TBaseState>
        where TBaseState : IState
    {
        public async Task EnterState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            await ((IStateWithAnimatedTransition)state).StateEnterAnimationHandler.Play(state);
        }

        public async Task ExitState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            await ((IStateWithAnimatedTransition)state).StateExitAnimationHandler.Play(state);
        }
    }
}