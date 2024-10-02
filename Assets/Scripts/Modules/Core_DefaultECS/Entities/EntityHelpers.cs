using System;

using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public static class EntityHelpers
    {
        public static bool TryGetRegistryEntity(
            Entity entity,
            EntityManager entityManager,
            out Entity registryEntity)
        {
            registryEntity = default;

            if (!entity.Has<GUIDComponent>())
                return false;

            var entityID = entity.Get<GUIDComponent>().GUID;

            return entityManager.TryGetRegistryEntity(
                entityID,
                out registryEntity);
        }

        public static bool TryGetSimulationEntity(
            Entity entity,
            EntityManager entityManager,
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
            EntityManager entityManager,
            out Entity simulationEntity)
        {
            simulationEntity = default;

            if (!entityManager.TryGetEntity(
                entityID,
                WorldConstants.SIMULATION_WORLD_ID,
                out simulationEntity))
                return false;

            //Additional check
            if (!simulationEntity.IsAlive)
            {
                return false;
            }

            return true;
        }

        public static bool TryGetViewEntity(
            Guid entityID,
            EntityManager entityManager,
            out Entity viewEntity)
        {
            viewEntity = default;

            if (!entityManager.TryGetEntity(
                entityID,
                WorldConstants.VIEW_WORLD_ID,
                out viewEntity))
                return false;

            if (!viewEntity.IsAlive)
            {
                return false;
            }

            return true;
        }
    }
}