using System.Threading.Tasks;

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
            byte preferredPlayerSlot = byte.MaxValue);

        void Disconnect();
    }
}