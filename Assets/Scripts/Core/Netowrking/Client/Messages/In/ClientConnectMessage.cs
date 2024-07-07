using HereticalSolutions.Messaging;

namespace HereticalSolutions.Networking
{
    public class ClientConnectMessage
        : IMessage
    {
        public string IP { get; private set; }

        public int Port { get; private set; }

        public string Secret { get; private set; }

        public byte PreferredPlayerSlot { get; private set; }

        public void Write(object[] args)
        {
            IP = (string)args[0];

            Port = (int)args[1];

            Secret = (string)args[2];

            PreferredPlayerSlot = (byte)args[3];
        }
    }
}