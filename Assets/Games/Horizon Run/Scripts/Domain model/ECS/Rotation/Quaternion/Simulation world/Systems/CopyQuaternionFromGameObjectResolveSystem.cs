using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
    public class CopyQuaternionFromGameObjectResolveSystem<TSceneEntity>
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

            if (!entity.Has<QuaternionComponent>())
                return;


            ref ResolveSimulationComponent resolveSimulationComponent = ref entity.Get<ResolveSimulationComponent>();

            ref QuaternionComponent quaternionComponent = ref entity.Get<QuaternionComponent>();


            GameObject source = resolveSimulationComponent.Source as GameObject;

            if (source == null)
                return;


            TransformQuaternionViewComponent quaternionViewComponent = source.GetComponentInChildren<TransformQuaternionViewComponent>();

            if (quaternionViewComponent == null)
                return;


            quaternionComponent.Quaternion = quaternionViewComponent.QuaternionPivotTransform.rotation;
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
