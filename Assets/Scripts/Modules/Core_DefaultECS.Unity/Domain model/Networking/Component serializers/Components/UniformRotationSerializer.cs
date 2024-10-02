using DefaultEcs;

using HereticalSolutions.Templates.Universal.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class UniformRotationSerializer
        : IComponentSerializer
    {
        public float Angle;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.SIMULATION;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Angle);
        }

        public void Deserialize(NetDataReader reader)
        {
            Angle = reader.GetFloat();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<UniformRotationComponent>())
                return false;

            var rotationComponent = entity.Get<UniformRotationComponent>();

            this.Angle = rotationComponent.Angle;

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (entity.Has<UniformRotationComponent>())
            {
                ref var rotationComponent = ref entity.Get<UniformRotationComponent>();

                rotationComponent.Angle = this.Angle;
            }
            else
            {
                entity.Set<UniformRotationComponent>(
                    new UniformRotationComponent
                    {
                        Angle = this.Angle
                    });
            }

            entity.Set<ServerUniformRotationComponent>(
                new ServerUniformRotationComponent
                {
                    ServerRotation = Angle
                });

            return true;
        }
        
        public void Clear()
        {
            Angle = default;
        }
    }
}