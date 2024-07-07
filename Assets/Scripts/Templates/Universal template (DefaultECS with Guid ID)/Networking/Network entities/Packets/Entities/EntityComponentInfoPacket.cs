using System;

using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    [Packet]
    public class EntityComponentInfoPacket
        : INetSerializable
    {
        public ushort Count;
        
        public ushort[] SerializerIDs;
        
        public IComponentSerializer[] Serializers;

        #region IContainsComponentData

        public IComponentSerializerManager ComponentSerializerManager { get; set; }

        #endregion
        
        public void Serialize(NetDataWriter writer)
        {
            if (ComponentSerializerManager == null)
            {
                throw new Exception(
                    "[EntityComponentInfoPacket] ComponentSerializerManager IS NULL");
            }
            
            writer.Put(Count);
            
            writer.PutArray(SerializerIDs);
            
            ComponentSerializerManager.InvokeComponentSerializersSerialization(
                Serializers,
                SerializerIDs,
                writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            if (ComponentSerializerManager == null)
            {
                throw new Exception(
                    "[EntityComponentInfoPacket] ComponentSerializerManager IS NULL");
            }
            
            Count = reader.GetUShort();

            SerializerIDs = reader.GetUShortArray();
            
            Serializers = new IComponentSerializer[Count];
            
            ComponentSerializerManager.InvokeComponentSerializersDeserialization(
                Serializers,
                SerializerIDs,
                reader);
        }
    }
}