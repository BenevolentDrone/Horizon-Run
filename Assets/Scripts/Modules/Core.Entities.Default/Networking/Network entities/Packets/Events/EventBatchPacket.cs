using System;

using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Packet]
    public class EventBatchPacket
        : IContainsComponentData
    {
        public ushort ServerTick;

        public ushort Count;

        public EventPacket[] Events;

        #region IContainsComponentData

        public IComponentSerializerManager ComponentSerializerManager { get; set; }

        #endregion
        
        public void Serialize(NetDataWriter writer)
        {
            if (ComponentSerializerManager == null)
            {
                throw new Exception(
                    "[EventBatchPacket] ComponentSerializerManager IS NULL");
            }
            
            writer.Put(ServerTick);
            
            writer.Put(Count);

            for (int i = 0; i < Count; i++)
            {
                ComponentSerializerManager.Serialize(
                    Events[i],
                    writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            if (ComponentSerializerManager == null)
            {
                throw new Exception(
                    "[EventBatchPacket] ComponentSerializerManager IS NULL");
            }
            
            ServerTick = reader.GetUShort();
            
            Count = reader.GetUShort();
            
            Events = new EventPacket[Count];

            for (int i = 0; i < Count; i++)
            {
                Events[i] = new EventPacket();
                
                ComponentSerializerManager.Deserialize(
                    Events[i],
                    reader);
            }
        }
    }
}