using System.Numerics;

using DefaultEcs;

using LiteNetLib.Utils;

using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class EventTargetPositionSerializer
        : IComponentSerializer
    {
        private Vector3 position;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.EVENT;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(position.X);
            writer.Put(position.Y);
            writer.Put(position.Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            float x = reader.GetFloat();
            float y = reader.GetFloat();
            float z = reader.GetFloat();
            
            position = new Vector3(x, y, z);
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<EventTargetPositionComponent>())
                return false;

            var eventTargetPositionComponent = entity.Get<EventTargetPositionComponent>();
            
            position = eventTargetPositionComponent.Position;
            
            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            entity.Set<EventTargetPositionComponent>(
                new EventTargetPositionComponent
                {
                    Position = position
                });

            return true;
        }

        public void Clear()
        {
            position = default;
        }
    }
}