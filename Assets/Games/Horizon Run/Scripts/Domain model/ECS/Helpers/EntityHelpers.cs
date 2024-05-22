using System;

using HereticalSolutions.Entities;

using UnityEngine;

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

        public static bool TryGetPosition(
            Entity entity,
            out Vector3 position)
        {
            position = Vector3.zero;
            
            if (entity.Has<PositionComponent>())
            {
                position = entity.Get<PositionComponent>().Position;

                return true;
            }

            if (entity.Has<NestedPositionComponent>())
            {
                var nestedPositionComponent = entity.Get<NestedPositionComponent>();

                var positionSourceEntity = nestedPositionComponent.PositionSourceEntity;

                if (!positionSourceEntity.IsAlive
                    || !positionSourceEntity.Has<PositionComponent>())
                    return false;

                position = nestedPositionComponent
                    .PositionSourceEntity
                    .Get<PositionComponent>()
                    .Position;
                
                if (nestedPositionComponent.LocalPosition.magnitude > MathHelpers.EPSILON
                    && positionSourceEntity.Has<UniformRotationComponent>())
                {
                    var angle = positionSourceEntity.Get<UniformRotationComponent>().Angle;

                    var rotation = Quaternion.Euler(0, angle, 0) 
                        * Quaternion.LookRotation(nestedPositionComponent.LocalPosition);
					
                    position += rotation
                        * Vector3.forward 
                        * nestedPositionComponent.LocalPosition.magnitude;
                }
                else if (nestedPositionComponent.LocalPosition.magnitude > MathHelpers.EPSILON
                    && positionSourceEntity.Has<QuaternionComponent>())
                {
                    var quaternion = positionSourceEntity.Get<QuaternionComponent>().Quaternion;

                    var rotation = quaternion
                        * Quaternion.LookRotation(nestedPositionComponent.LocalPosition);

                    position += rotation
                        * Vector3.forward
                        * nestedPositionComponent.LocalPosition.magnitude;
                }
                else
                {
                    position += nestedPositionComponent.LocalPosition;
                }

                return true;
            }

            return false;
        }
    }
}