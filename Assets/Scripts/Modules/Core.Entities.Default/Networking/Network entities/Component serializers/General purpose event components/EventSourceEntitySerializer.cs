using System;

using HereticalSolutions.Entities;

using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class EventSourceEntitySerializer
        : IComponentSerializer
    {
        public ushort SourceNetworkID;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SourceNetworkID);
        }

        public void Deserialize(NetDataReader reader)
        {
            SourceNetworkID = reader.GetUShort();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventSourceEntityComponent<Guid>>())
                return false;

            var eventReceiverEntityComponent = entity.Get<EventSourceEntityComponent<Guid>>();

            if (!NetworkEntityHelpers.TryGetNetworkID<EventSourceEntitySerializer>(
                eventReceiverEntityComponent.SourceID,
                context,
                out SourceNetworkID))
            {
                SourceNetworkID = ushort.MaxValue;
            }

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!NetworkEntityHelpers.TryGetEntityID<EventSourceEntitySerializer>(
                SourceNetworkID,
                context,
                out var sourceEntityID))
            {
                sourceEntityID = Guid.Empty;
            }
            
            entity.Set<EventSourceEntityComponent<Guid>>(
                new EventSourceEntityComponent<Guid>
                {
                    SourceID = sourceEntityID
                });

            return true;
        }
        
        public void Clear()
        {
            SourceNetworkID = default;
        }
    }
}