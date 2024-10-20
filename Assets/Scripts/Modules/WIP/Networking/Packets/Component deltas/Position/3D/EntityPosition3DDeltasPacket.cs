using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    /*
    [Packet]
    public struct EntityPosition3DDeltasPacket : INetSerializable
    {
        public const int MAX_CAPACITY = (int)((NetworkingConsts.MAX_PAYLOAD_BYTES - 4) / 14);
        
        //2 bytes
        public ushort ServerTick;
        
        //2 bytes
        public ushort Count;
        
        public IDPosition3DPair[] Positions;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ServerTick);
            
            writer.Put(Count);
            
            for (int i = 0; i < Count; i++)
                Positions[i].Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            ServerTick = reader.GetUShort();
            
            Count = reader.GetUShort();
            
            Positions = new IDPosition3DPair[Count];

            for (int i = 0; i < Count; i++)
                Positions[i].Deserialize(reader);
        }
    }
    */
}