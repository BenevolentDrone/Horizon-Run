using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
    public interface ITransitionController<TBaseState>
        where TBaseState : IState
    {
        Task EnterState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ExitState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}