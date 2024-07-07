using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class ServerQuaternionInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;
        
        private bool isServer;
        
        public ServerQuaternionInitializationSystem(
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

            if (!entity.Has<QuaternionComponent>())
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
            
            if (entity.Has<ServerQuaternionComponent>())
                return;

            if (isServer)
            {
                entity.Set<ServerQuaternionComponent>(
                    new ServerQuaternionComponent
                    {
                        ServerQuaternion = entity.Get<QuaternionComponent>().Quaternion,
                        
                        ServerTick = 0,
                        
                        Dirty = false
                    });
            }
            else
            {
                entity.Set<ServerQuaternionComponent>();
            }
        }

        public void Dispose()
        {
        }
    }
}