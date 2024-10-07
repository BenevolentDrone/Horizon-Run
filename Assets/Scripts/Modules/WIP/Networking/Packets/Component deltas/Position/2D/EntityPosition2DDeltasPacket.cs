using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    [Packet]
    public struct EntityPosition2DDeltasPacket : INetSerializable
    {
        public const int MAX_CAPACITY = (int)((NetworkingConsts.MAX_PAYLOAD_BYTES - 4) / 10);
        
        //2 bytes
        public ushort ServerTick;
        
        //2 bytes
        public ushort Count;
        
        public IDPosition2DPair[] Positions;
        
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
            
            Positions = new IDPosition2DPair[Count];

            for (int i = 0; i < Count; i++)
                Positions[i].Deserialize(reader);
        }
    }
}