using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    /*
    [Packet]
    public struct EntityQuaternionDeltasPacket : INetSerializable
    {
        public const int MAX_CAPACITY = (int)((NetworkingConsts.MAX_PAYLOAD_BYTES - 4) / 18);
        
        //2 bytes
        public ushort ServerTick;
        
        //2 bytes
        public ushort Count;
        
        public IDQuaternionPair[] Quaternions;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ServerTick);
            
            writer.Put(Count);
            
            for (int i = 0; i < Count; i++)
                Quaternions[i].Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            ServerTick = reader.GetUShort();
            
            Count = reader.GetUShort();
            
            Quaternions = new IDQuaternionPair[Count];

            for (int i = 0; i < Count; i++)
                Quaternions[i].Deserialize(reader);
        }
    }
    */
}