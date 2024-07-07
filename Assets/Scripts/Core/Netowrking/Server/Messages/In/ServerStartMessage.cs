using HereticalSolutions.Messaging;

namespace HereticalSolutions.Networking
{
    public class ServerStartMessage
        : IMessage
    {
        public int Port { get; private set; }

        public bool ReserveSlotForSelf { get; private set; }

        public void Write(object[] args)
        {
            Port = (int)args[0];

            ReserveSlotForSelf = (bool)args[1];
        }
    }
}