namespace HereticalSolutions.Networking
{
    public struct ServerToClientConnectionDescriptor
    {
        public EServerToClientConnectionStatus Status;
        
        public int PeerID;
        
        
        public string ClientIP;

        public int ClientPort;
        
        public int Ping;
        
        
        public string Username;
    }
}