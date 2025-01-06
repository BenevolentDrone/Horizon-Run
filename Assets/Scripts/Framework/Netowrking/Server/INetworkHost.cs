using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Networking
{
    public interface INetworkHost
    {
        EHostStatus Status { get; }

        ushort Tick { get; }

        int ActiveConnectionsCount { get; }

        ServerToClientConnectionDescriptor[] Connections { get; }

        Task Start(
            int port,
            bool reserveSlotForSelf = false,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task Stop(
            
            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}