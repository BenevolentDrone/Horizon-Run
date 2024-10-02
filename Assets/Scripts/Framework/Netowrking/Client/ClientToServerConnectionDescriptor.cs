namespace HereticalSolutions.Networking
{
    public struct ClientToServerConnectionDescriptor
    {
        public string ServerIP;

        public int ServerPort;

        public int Ping;

        public byte PlayerSlot;
    }
}