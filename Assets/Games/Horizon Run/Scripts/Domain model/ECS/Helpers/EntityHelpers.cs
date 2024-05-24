using System;

using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
    public static class EntityHelpers
    {
        public static bool TryGetRegistryEntity(
            Entity entity,
            HorizonRunEntityManager entityManager,
            out Entity registryEntity)
        {
            registryEntity = default;
            
            if (!entity.Has<GUIDComponent>())
                return false;
            
            var entityID = entity.Get<GUIDComponent>().GUID;

            if (!entityManager.HasEntity(entityID))
                return false;
            
            registryEntity = entityManager.GetRegistryEntity(
                entityID);

            return true;
        }
        
        public static bool TryGetSimulationEntity(
            Entity entity,
            HorizonRunEntityManager entityManager,
            out Entity simulationEntity)
        {
            simulationEntity = default;
            
            if (!entity.Has<GUIDComponent>())
                return false;
            
            var entityID = entity.Get<GUIDComponent>().GUID;

            return TryGetSimulationEntity(
                entityID,
                entityManager,
                out simulationEntity);
        }
        
        public static bool TryGetSimulationEntity(
            Guid entityID,
            HorizonRunEntityManager entityManager,
            out Entity simulationEntity)
        {
            simulationEntity = default;
            
            simulationEntity = entityManager.GetEntity(
                entityID,
                WorldConstants.SIMULATION_WORLD_ID);

            if (!simulationEntity.IsAlive)
            {
                return false;
            }

            return true;
        }
        
        public static bool TryGetViewEntity(
            Guid entityID,
            HorizonRunEntityManager entityManager,
            out Entity viewEntity)
        {
            viewEntity = default;
            
            viewEntity = entityManager.GetEntity(
                entityID,
                WorldConstants.VIEW_WORLD_ID);

            if (!viewEntity.IsAlive)
            {
                return false;
            }

            return true;
        }
    }
}