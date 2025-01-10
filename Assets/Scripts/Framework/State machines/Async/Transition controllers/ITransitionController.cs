using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StateMachines
{
    public interface ITransitionController<TBaseState>
        where TBaseState : IState
    {
        Task EnterState(
            TBaseState state,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task ExitState(
            TBaseState state,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}