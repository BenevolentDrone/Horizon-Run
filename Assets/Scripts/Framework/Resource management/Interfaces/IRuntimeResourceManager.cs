using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    public interface IRuntimeResourceManager
        : IReadOnlyRuntimeResourceManager
    {
        Task AddRootResource(
            IReadOnlyResourceData rootResource,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveRootResource(
            int rootResourceIDHash = -1,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveRootResource(
            string rootResourceID,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task ClearAllRootResources(
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}