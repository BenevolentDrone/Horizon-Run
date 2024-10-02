using System;

using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Packet]
    public class EntityCreatedPacket
        : IContainsComponentData
    {
        public ushort ServerTick;
        
        public ushort NetworkID;

        public byte AuthoringPermission;
        
        public byte[] GUID;
        
        public ushort PrototypeID;
        
        
        public ushort ComponentDataCount;
        
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
                    "[EntityCreatedPacket] ComponentSerializerManager IS NULL");
            }
            
            writer.Put(ServerTick);
            
            writer.Put(NetworkID);
            
            writer.Put(AuthoringPermission);
            
            writer.PutBytesWithLength(GUID);
            
            writer.Put(PrototypeID);
            
            
            writer.Put(ComponentDataCount);
            
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
                    "[EntityCreatedPacket] ComponentSerializerManager IS NULL");
            }
            
            ServerTick = reader.GetUShort();
            
            NetworkID = reader.GetUShort();

            AuthoringPermission = reader.GetByte();
            
            GUID = reader.GetBytesWithLength();
            
            PrototypeID = reader.GetUShort();
            
            
            ComponentDataCount = reader.GetUShort();

            SerializerIDs = reader.GetUShortArray();
            
            Serializers = new IComponentSerializer[ComponentDataCount];
            
            ComponentSerializerManager.InvokeComponentSerializersDeserialization(
                Serializers,
                SerializerIDs,
                reader);
        }
    }
}