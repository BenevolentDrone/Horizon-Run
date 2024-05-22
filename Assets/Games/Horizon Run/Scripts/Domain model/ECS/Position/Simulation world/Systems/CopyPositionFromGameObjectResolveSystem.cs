using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
    public class CopyPositionFromGameObjectResolveSystem<TSceneEntity>
        : IDefaultECSEntityInitializationSystem
          where TSceneEntity : MonoBehaviour
    {
        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<ResolveSimulationComponent>())
                return;

            if (!entity.Has<PositionComponent>())
                return;


            ref ResolveSimulationComponent resolveSimulationComponent = ref entity.Get<ResolveSimulationComponent>();

            ref PositionComponent positionComponent = ref entity.Get<PositionComponent>();


            GameObject source = resolveSimulationComponent.Source as GameObject;

            if (source == null)
                return;


            var worldPosition = source.transform.position;


            TransformPositionViewComponent positionViewComponent = source.GetComponentInChildren<TransformPositionViewComponent>();

            if (positionViewComponent != null)
            {
                worldPosition = positionViewComponent.PositionTransform.position;
            
            }

            positionComponent.Position = worldPosition;
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
