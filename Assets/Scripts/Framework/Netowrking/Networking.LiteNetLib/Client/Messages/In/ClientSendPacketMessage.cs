using System;

using HereticalSolutions.Messaging;

using LiteNetLib;
using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class ClientSendPacketMessage
        : IMessage
    {
        public Type PacketType { get; private set; }
        
        public INetSerializable Packet { get; private set; }
        
        public Action<INetSerializable, NetDataWriter> PacketSerializationAction { get; private set; }
        
        public DeliveryMethod DeliveryMethod { get; private set; }
        
        public void Write(object[] args)
        {
            PacketType = (Type)args[0];
            
            Packet = (INetSerializable)args[1];
            
            PacketSerializationAction = (Action<INetSerializable, NetDataWriter>)args[2];
            
            DeliveryMethod = (DeliveryMethod)args[3];
        }
    }
}