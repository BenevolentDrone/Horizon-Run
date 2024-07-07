using System.Threading.Tasks;

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
            bool reserveSlotForSelf = false);

        Task Stop();
    }
}