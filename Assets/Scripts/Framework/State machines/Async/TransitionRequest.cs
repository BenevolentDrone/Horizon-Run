using System;
using System.Threading;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StateMachines
{
    public class TransitionRequest<TBaseState>
        where TBaseState : IState
    {
        public ETransitionState TransitionState = ETransitionState.QUEUED;

        public ITransitionEvent<TBaseState> Event;

        public TransitionSupervisor TransitionController;


        public IProgress<float> StateExitProgress;

        public IProgress<float> StateEnterProgress;


        public CancellationTokenSource CancellationTokenSource;

        public IProgress<float> Progress;

        public ILogger ProgressLogger;
    }
}