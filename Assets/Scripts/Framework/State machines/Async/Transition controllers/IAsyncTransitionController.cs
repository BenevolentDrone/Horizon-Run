using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
    public interface IAsyncTransitionController<TBaseState>
        where TBaseState : IAsyncState
    {
        Task EnterState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task EnterState(
            TBaseState state,

            IAsyncTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ExitState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ExitState(
            TBaseState state,

            IAsyncTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}