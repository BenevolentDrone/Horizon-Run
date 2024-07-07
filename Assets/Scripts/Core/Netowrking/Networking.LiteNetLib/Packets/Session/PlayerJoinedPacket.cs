using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    [Packet]
    public class PlayerJoinedPacket : INetSerializable
    {
        public byte PlayerSlot;

        public string Username;

        public bool Rejoin;

        public ushort ServerTick;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerSlot);

            writer.Put(Username);

            writer.Put(Rejoin);

            writer.Put(ServerTick);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerSlot = reader.GetByte();

            Username = reader.GetString();

            Rejoin = reader.GetBool();

            ServerTick = reader.GetUShort();
        }
    }
}