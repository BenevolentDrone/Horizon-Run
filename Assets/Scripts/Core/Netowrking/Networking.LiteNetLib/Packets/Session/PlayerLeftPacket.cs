using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    [Packet]
    public class PlayerLeftPacket : INetSerializable
    {
        public byte PlayerSlot;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerSlot);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerSlot = reader.GetByte();
        }
    }
}