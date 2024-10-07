using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
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