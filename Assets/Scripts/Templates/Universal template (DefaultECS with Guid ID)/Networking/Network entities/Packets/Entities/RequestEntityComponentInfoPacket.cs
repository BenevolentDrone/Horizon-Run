using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    [Packet]
    public struct RequestEntityComponentInfoPacket : INetSerializable
    {
        public ushort NetworkID;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NetworkID);
        }

        public void Deserialize(NetDataReader reader)
        {
            NetworkID = reader.GetUShort();
        }
    }
}