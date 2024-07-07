using System;

using HereticalSolutions.Entities;

using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public class EventReceiverEntitySerializer
        : IComponentSerializer
    {
        public ushort ReceiverNetworkID;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ReceiverNetworkID);
        }

        public void Deserialize(NetDataReader reader)
        {
            ReceiverNetworkID = reader.GetUShort();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventReceiverEntityComponent<Guid>>())
                return false;

            var eventReceiverEntityComponent = entity.Get<EventReceiverEntityComponent<Guid>>();

            if (!NetworkEntityHelpers.TryGetNetworkID<EventReceiverEntitySerializer>(
                eventReceiverEntityComponent.ReceiverID,
                context,
                out ReceiverNetworkID))
            {
                ReceiverNetworkID = ushort.MaxValue;
            }

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!NetworkEntityHelpers.TryGetEntityID<EventReceiverEntitySerializer>(
                ReceiverNetworkID,
                context,
                out var receiverEntityID))
            {
                receiverEntityID = Guid.Empty;
            }
            
            entity.Set<EventReceiverEntityComponent<Guid>>(
                new EventReceiverEntityComponent<Guid>
                {
                    ReceiverID = receiverEntityID
                });

            return true;
        }
        
        public void Clear()
        {
            ReceiverNetworkID = default;
        }
    }
}