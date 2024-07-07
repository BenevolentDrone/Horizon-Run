using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    [Packet]
    public class JoinConfirmedPacket : INetSerializable
    {
        public byte PlayerSlot;

        public ushort ServerTick;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerSlot);

            writer.Put(ServerTick);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerSlot = reader.GetByte();

            ServerTick = reader.GetUShort();
        }
    }
}