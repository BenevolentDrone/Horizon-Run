using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

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

            //Async tail
            AsyncExecutionContext asyncContext,
            
            byte preferredPlayerSlot = byte.MaxValue); //TODO: refactor

        void Disconnect();
    }
}