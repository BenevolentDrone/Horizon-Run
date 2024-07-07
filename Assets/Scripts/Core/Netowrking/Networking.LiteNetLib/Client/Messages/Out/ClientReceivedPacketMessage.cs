using System;

using HereticalSolutions.Messaging;

using LiteNetLib;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class ClientReceivedPacketMessage
        : IMessage
    {
        public Type PacketType { get; private set; }
        
        public NetPacketReader Reader { get; private set; }
        

        public byte ChannelNumber { get; private set; }
        
        public DeliveryMethod DeliveryMethod { get; private set; }
        
        
        public void Write(object[] args)
        {
            PacketType = (Type)args[0];
            
            Reader = (NetPacketReader)args[1];
            
            
            ChannelNumber = (byte)args[2];
            
            DeliveryMethod = (DeliveryMethod)args[3];
        }
    }
}