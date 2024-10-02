using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class CopyQuaternionFromGameObjectResolveSystem<TSceneEntity>
        : IEntityInitializationSystem
        where TSceneEntity : MonoBehaviour
    {
        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<ResolveComponent>())
                return;

            if (!entity.Has<QuaternionComponent>())
                return;


            ref ResolveComponent ResolveComponent = ref entity.Get<ResolveComponent>();

            ref QuaternionComponent quaternionComponent = ref entity.Get<QuaternionComponent>();


            GameObject source = ResolveComponent.Source as GameObject;

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
