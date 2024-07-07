using DefaultEcs;

using HereticalSolutions.Templates.Universal.Networking;

using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class QuaternionSerializer
        : IComponentSerializer
    {
        public Quaternion Quaternion;
        
        public ESerializedEntityType SerializedEntityType
        {
            get => ESerializedEntityType.SIMULATION;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Quaternion);
        }

        public void Deserialize(NetDataReader reader)
        {
            Quaternion = reader.GetQuaternion();
        }

        public bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (!entity.Has<QuaternionComponent>())
                return false;

            var quaternionComponent = entity.Get<QuaternionComponent>();

            this.Quaternion = quaternionComponent.Quaternion;

            return true;
        }

        public bool WriteTo(
            Entity entity,
            ComponentSerializationContext context)
        {
            if (entity.Has<QuaternionComponent>())
            {
                ref var quaternionComponent = ref entity.Get<QuaternionComponent>();

                quaternionComponent.Quaternion = this.Quaternion;
            }
            else
            {
                entity.Set<QuaternionComponent>(
                    new QuaternionComponent
                    {
                        Quaternion = this.Quaternion
                    });
            }

            entity.Set<ServerQuaternionComponent>(
                new ServerQuaternionComponent
                {
                    ServerQuaternion = this.Quaternion
                });

            return true;
        }
        
        public void Clear()
        {
            Quaternion = default;
        }
    }
}