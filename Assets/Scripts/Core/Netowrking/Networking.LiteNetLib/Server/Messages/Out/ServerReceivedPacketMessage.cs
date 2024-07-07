using System;

using HereticalSolutions.Messaging;

using LiteNetLib;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class ServerReceivedPacketMessage
        : IMessage
    {
        public byte PlayerSlot { get; private set; }

        public NetPeer Peer { get; private set; }


        public Type PacketType { get; private set; }

        public NetPacketReader Reader { get; private set; }


        public byte ChannelNumber { get; private set; }

        public DeliveryMethod DeliveryMethod { get; private set; }

        public void Write(object[] args)
        {
            PlayerSlot = (byte)args[0];

            Peer = (NetPeer)args[1];


            PacketType = (Type)args[2];

            Reader = (NetPacketReader)args[3];


            ChannelNumber = (byte)args[4];

            DeliveryMethod = (DeliveryMethod)args[5];
        }
    }
}