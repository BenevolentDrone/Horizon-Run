using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    public interface IAllocatable
    {
        bool Allocated { get; }

        Task Allocate(

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task Free(
            
            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}