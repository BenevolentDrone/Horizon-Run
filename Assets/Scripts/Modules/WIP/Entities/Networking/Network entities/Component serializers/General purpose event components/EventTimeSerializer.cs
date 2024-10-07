using DefaultEcs;

using HereticalSolutions.Entities;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class EventTimeSerializer
        : IComponentSerializer
    {
        private long ticks;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ticks);
        }

        public void Deserialize(NetDataReader reader)
        {
            ticks = reader.GetLong();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventTimeComponent>())
                return false;

            var eventTimeComponent = entity.Get<EventTimeComponent>();
            
            ticks = eventTimeComponent.Ticks;
            
            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            entity.Set<EventTimeComponent>(
                new EventTimeComponent
                {
                    Ticks = ticks
                });

            return true;
        }
        
        public void Clear()
        {
            ticks = default;
        }
    }
}