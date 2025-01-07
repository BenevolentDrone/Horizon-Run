using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public interface IPoolElementFacade<T>
    {
        T Value { get; set; }

        EPoolElementStatus Status { get; set; }

        IManagedPool<T> Pool { get; set; }

        Task Push(

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}