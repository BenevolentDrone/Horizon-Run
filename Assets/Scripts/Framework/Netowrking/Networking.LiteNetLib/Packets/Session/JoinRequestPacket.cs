using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    [Packet]
    public class JoinRequestPacket : INetSerializable
    {
        public string Username;

        public string PlayerId;

        public byte PreferredPlayerSlot;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Username);

            writer.Put(PlayerId);

            writer.Put(PreferredPlayerSlot);
        }

        public void Deserialize(NetDataReader reader)
        {
            Username = reader.GetString();

            PlayerId = reader.GetString();

            PreferredPlayerSlot = reader.GetByte();
        }
    }
}