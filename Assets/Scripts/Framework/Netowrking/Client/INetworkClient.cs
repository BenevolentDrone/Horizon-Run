using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Networking
{
    public interface INetworkClient
    {
        EClientStatus Status { get; }

        ClientToServerConnectionDescriptor Connection { get; }

        void Start();

        void Stop();

        Task Connect(
            string ip,
            int port,
            string secret,
            byte preferredPlayerSlot = byte.MaxValue, //TODO: refactor

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        void Disconnect();
    }
}