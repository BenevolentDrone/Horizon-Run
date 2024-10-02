using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Packet]
    public struct RequestWorldInfoPacket : INetSerializable
    {
        public void Serialize(NetDataWriter writer)
        {
        }

        public void Deserialize(NetDataReader reader)
        {
        }
    }
}