using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    /*
    public class ServerPosition3DInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;

        private readonly ILogger logger;
        
        private bool isServer;
        
        public ServerPosition3DInitializationSystem(
            UniversalTemplateEntityManager entityManager,
            bool isServer,
            ILogger logger = null)
        {
            this.entityManager = entityManager;
            
            this.isServer = isServer;
            
            this.logger = logger;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<Position3DComponent>())
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

            if (entity.Has<ServerPosition3DComponent>())
                return;
            
            if (isServer)
            {
                entity.Set<ServerPosition3DComponent>(
                    new ServerPosition3DComponent
                    {
                        ServerPosition = entity.Get<Position3DComponent>().Position,
                        
                        ServerTick = 0,
                        
                        Dirty = false
                    });
            }
            else
            {
                entity.Set<ServerPosition3DComponent>();
            }
        }

        public void Dispose()
        {
        }
    }
    */
}