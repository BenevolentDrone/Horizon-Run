using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class CopyPosition3DFromGameObjectResolveSystem<TSceneEntity>
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

            if (!entity.Has<Position3DComponent>())
                return;


            ref ResolveComponent ResolveComponent = ref entity.Get<ResolveComponent>();

            ref Position3DComponent position3DComponent = ref entity.Get<Position3DComponent>();


            GameObject source = ResolveComponent.Source as GameObject;

            if (source == null)
                return;


            var worldPosition = source.transform.position;


            TransformPosition3DViewComponent positionViewComponent = source.GetComponentInChildren<TransformPosition3DViewComponent>();

            if (positionViewComponent != null)
            {
                worldPosition = positionViewComponent.PositionTransform.position;
            
            }

            position3DComponent.Position = worldPosition;

            if (entity.Has<Transform3DComponent>())
            {
                ref Transform3DComponent transformComponent = ref entity.Get<Transform3DComponent>();

                transformComponent.Dirty = true;
            }
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
