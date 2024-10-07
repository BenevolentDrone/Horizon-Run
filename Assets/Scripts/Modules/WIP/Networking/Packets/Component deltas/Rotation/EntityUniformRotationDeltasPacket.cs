using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    [Packet]
    public struct EntityUniformRotationDeltasPacket : INetSerializable
    {
        public const int MAX_CAPACITY = (int)((NetworkingConsts.MAX_PAYLOAD_BYTES - 4) / 6);
        
        //2 bytes
        public ushort ServerTick;
        
        //2 bytes
        public ushort Count;
        
        public IDUniformRotationPair[] UniformRotations;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ServerTick);
            
            writer.Put(Count);
            
            for (int i = 0; i < Count; i++)
                UniformRotations[i].Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            ServerTick = reader.GetUShort();
            
            Count = reader.GetUShort();
            
            UniformRotations = new IDUniformRotationPair[Count];

            for (int i = 0; i < Count; i++)
                UniformRotations[i].Deserialize(reader);
        }
    }
}