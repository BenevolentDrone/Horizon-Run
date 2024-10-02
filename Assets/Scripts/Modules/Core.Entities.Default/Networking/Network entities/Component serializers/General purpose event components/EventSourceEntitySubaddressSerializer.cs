using HereticalSolutions.Entities;

using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class EventSourceEntitySubaddressSerializer
        : IComponentSerializer
    {
        public ushort[] Subaddress;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.PutArray(Subaddress);
        }

        public void Deserialize(NetDataReader reader)
        {
            Subaddress = reader.GetUShortArray();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventSourceEntitySubaddressComponent>())
                return false;

            var eventSourceEntitySubaddressComponent = entity.Get<EventSourceEntitySubaddressComponent>();

            Subaddress = eventSourceEntitySubaddressComponent.Subaddress;

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            entity.Set<EventSourceEntitySubaddressComponent>(
                new EventSourceEntitySubaddressComponent
                {
                    Subaddress = this.Subaddress
                });

            return true;
        }
        
        public void Clear()
        {
            Subaddress = default;
        }
    }
}