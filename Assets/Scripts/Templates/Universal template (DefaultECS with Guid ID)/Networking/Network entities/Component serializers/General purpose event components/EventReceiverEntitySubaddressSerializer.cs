using HereticalSolutions.Entities;

using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public class EventReceiverEntitySubaddressSerializer
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
            if (!entity.Has<EventReceiverEntitySubaddressComponent>())
                return false;

            var eventReceiverEntitySubaddressComponent = entity.Get<EventReceiverEntitySubaddressComponent>();

            Subaddress = eventReceiverEntitySubaddressComponent.Subaddress;

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            entity.Set<EventReceiverEntitySubaddressComponent>(
                new EventReceiverEntitySubaddressComponent
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