using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

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
            AsyncExecutionContext asyncContext);

        Task Stop(

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}