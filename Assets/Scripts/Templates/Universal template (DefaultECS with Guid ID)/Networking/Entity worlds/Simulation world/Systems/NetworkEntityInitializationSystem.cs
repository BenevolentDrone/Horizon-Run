using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public class NetworkEntityInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;
        
        private readonly INetworkEntityRepository<Guid> networkEntityRepository;
        
        private readonly bool isServerOrHost;

        private readonly ILogger logger;
        
        public NetworkEntityInitializationSystem(
            UniversalTemplateEntityManager entityManager,
            INetworkEntityRepository<Guid> networkEntityRepository,
            bool isServerOrHost,
            ILogger logger = null)
        {
            this.entityManager = entityManager;

            this.networkEntityRepository = networkEntityRepository;
            
            this.isServerOrHost = isServerOrHost;
            
            this.logger = logger;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;
            
            if (!entity.Has<GUIDComponent>())
                return;
            
            var entityID = entity.Get<GUIDComponent>().GUID;


            var registryEntity = entityManager.GetRegistryEntity(
                entityID);

            if (!registryEntity.Has<NetworkEntityComponent>())
            {
                return;
            }

            ref var networkEntityComponent = ref registryEntity.Get<NetworkEntityComponent>();
            
            if (isServerOrHost)
            {
                networkEntityRepository.TryAllocateNetworkEntityID(out var networkID);

                networkEntityComponent.NetworkID = networkID;

                if (!networkEntityRepository.TryAddNetworkEntity(
                    networkID,
                    entityID))
                {
                    networkEntityComponent.NetworkID = ushort.MaxValue;
                    
                    //return;
                }
            }
            else
            {
                networkEntityComponent.NetworkID = ushort.MaxValue;
            }
        }

        public void Dispose()
        {
        }
    }
}