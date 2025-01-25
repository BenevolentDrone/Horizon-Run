/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ResourceManagement
{
    public interface IRuntimeResourceManager
        : IReadOnlyRuntimeResourceManager
    {
        Task AddRootResource(
            IReadOnlyResourceData rootResource,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveRootResource(
            int rootResourceIDHash = -1,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveRootResource(
            string rootResourceID,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ClearAllRootResources(
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}
*/