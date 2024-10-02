using System;

using HereticalSolutions.Entities;

using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class EventTargetEntitySerializer
        : IComponentSerializer
    {
        public ushort TargetNetworkID;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(TargetNetworkID);
        }

        public void Deserialize(NetDataReader reader)
        {
            TargetNetworkID = reader.GetUShort();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventTargetEntityComponent<Guid>>())
                return false;

            var eventReceiverEntityComponent = entity.Get<EventTargetEntityComponent<Guid>>();

            if (!NetworkEntityHelpers.TryGetNetworkID<EventTargetEntitySerializer>(
                eventReceiverEntityComponent.TargetID,
                context,
                out TargetNetworkID))
            {
                return false;
            }

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!NetworkEntityHelpers.TryGetEntityID<EventTargetEntitySerializer>(
                TargetNetworkID,
                context,
                out var targetEntityID))
            {
                return false;
            }
            
            entity.Set<EventTargetEntityComponent<Guid>>(
                new EventTargetEntityComponent<Guid>
                {
                    TargetID = targetEntityID
                });

            return true;
        }
        
        public void Clear()
        {
            TargetNetworkID = default;
        }
    }
}