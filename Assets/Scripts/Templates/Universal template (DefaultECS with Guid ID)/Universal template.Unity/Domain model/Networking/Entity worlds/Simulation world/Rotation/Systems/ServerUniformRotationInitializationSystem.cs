using HereticalSolutions.Entities;

using DefaultEcs;

using HereticalSolutions.Networking.ECS;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class ServerUniformRotationInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;
        
        private bool isServer;
        
        public ServerUniformRotationInitializationSystem(
            UniversalTemplateEntityManager entityManager,
            bool isServer)
        {
            this.entityManager = entityManager;
            
            this.isServer = isServer;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<UniformRotationComponent>())
                return;
            
            if (!EntityHelpers.TryGetRegistryEntity(
                entity,
                entityManager,
                out var registryEntity))
            {
                return;
            }

            if (!registryEntity.Has<NetworkEntityComponent>())
            {
                return;
            }

            if (entity.Has<ServerUniformRotationComponent>())
                return;
            
            if (isServer)
            {
                entity.Set<ServerUniformRotationComponent>(
                    new ServerUniformRotationComponent
                    {
                        ServerRotation = entity.Get<UniformRotationComponent>().Angle,
                        
                        ServerTick = 0,
                        
                        Dirty = false
                    });
            }
            else
            {
                entity.Set<ServerUniformRotationComponent>();
            }
        }

        public void Dispose()
        {
        }
    }
}