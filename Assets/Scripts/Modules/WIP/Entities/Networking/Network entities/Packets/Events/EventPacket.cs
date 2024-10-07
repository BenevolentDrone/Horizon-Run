using System;

using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Packet]
    public class EventPacket
        : IContainsComponentData
    {
        public byte PlayerSlot;
        
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
                    "[EventPacket] ComponentSerializerManager IS NULL");
            }
            
            writer.Put(PlayerSlot);
            
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
                    "[EventPacket] ComponentSerializerManager IS NULL");
            }
            
            PlayerSlot = reader.GetByte();
            
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