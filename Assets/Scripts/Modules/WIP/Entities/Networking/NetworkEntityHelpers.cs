using System;

using HereticalSolutions.Networking.ECS;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public static class NetworkEntityHelpers
    {
        public static bool TryGetNetworkID(
            Entity entity,
            EntityManager entityManager,
            out ushort networkID)
        {
            networkID = ushort.MaxValue;

            if (!EntityHelpers.TryGetRegistryEntity(
                entity,
                entityManager,
                out var registryEntity))
            {
                return false;
            }

            networkID = registryEntity.Get<NetworkEntityComponent>().NetworkID;
            
            return true;
        }
        public static bool TryGetNetworkID(
            Guid entityID,
            EntityManager entityManager,
            out ushort networkID)
        {
            networkID = ushort.MaxValue;

            if (!EntityHelpers.TryGetSimulationEntity(
                entityID,
                entityManager,
                out var entity))
            {
                return false;
            }
            
            if (!EntityHelpers.TryGetRegistryEntity(
                entity,
                entityManager,
                out var registryEntity))
            {
                return false;
            }

            networkID = registryEntity.Get<NetworkEntityComponent>().NetworkID;
            
            return true;
        }
        
        public static bool TryGetNetworkID<TLogSource>(
            Guid entityID,
            ComponentSerializationContext context,
            out ushort networkID)
        {
            if (entityID == Guid.Empty)
            {
                networkID = ushort.MaxValue;
                
                return false;
            }
            
            if (!context.NetworkEntityRepository.TryGetNetworkID(
                entityID,
                out networkID))
            {
                var logger = context.LoggerResolver?.GetLogger<TLogSource>();
                
                logger?.LogError<TLogSource>(
                    $"ENTITY {entityID} IS NOT A NETWORK ENTITY");
                
                return false;
            }

            return true;
        }

        public static bool TryGetEntityID<TLogSource>(
            ushort networkID,
            ComponentSerializationContext context,
            out Guid entityID)
        {
            if (networkID == ushort.MaxValue)
            {
                entityID = Guid.Empty;

                return false;
            }

            if (!context.NetworkEntityRepository.TryGetEntityID(
                networkID,
                out entityID))
            {
                var logger = context.LoggerResolver?.GetLogger<TLogSource>();

                logger?.LogError<TLogSource>(
                    $"NETWORK ENTITY {networkID} IS NOT FOUND");

                return false;
            }

            return true;
        }

        public static bool TryGetEntityID<TLogSource>(
            ushort networkID,
            INetworkEntityRepository<Guid> manager,
            out Guid entityID)
        {
            if (networkID == ushort.MaxValue)
            {
                entityID = Guid.Empty;
                
                return false;
            }
            
            if (!manager.TryGetEntityID(
                networkID,
                out entityID))
            {
                return false;
            }

            return true;
        }
    }
}