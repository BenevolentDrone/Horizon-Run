using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Packet]
    public struct NetworkEntityRollCallPacket : INetSerializable
    {
        public ushort ServerTick;

        public ushort[] NetworkIDs;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ServerTick);
            
            writer.PutArray(NetworkIDs);
        }

        public void Deserialize(NetDataReader reader)
        {
            ServerTick = reader.GetUShort();
            
            NetworkIDs = reader.GetUShortArray();
        }
    }
}