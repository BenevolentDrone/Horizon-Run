using System;

using HereticalSolutions.Messaging;

using LiteNetLib;
using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class ServerSendPacketMessage
        : IMessage
    {
        public byte PlayerSlot { get; private set; }

        public Type PacketType { get; private set; }

        public INetSerializable Packet { get; private set; }

        public Action<INetSerializable, NetDataWriter> PacketSerializationAction { get; private set; }

        public DeliveryMethod DeliveryMethod { get; private set; }

        public void Write(object[] args)
        {
            PlayerSlot = (byte)args[0];

            PacketType = (Type)args[1];

            Packet = (INetSerializable)args[2];

            PacketSerializationAction = (Action<INetSerializable, NetDataWriter>)args[3];

            DeliveryMethod = (DeliveryMethod)args[4];
        }
    }
}