using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class CopyPosition2DFromGameObjectResolveSystem<TSceneEntity>
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

			if (!entity.Has<Position2DComponent>())
				return;


			ref ResolveSimulationComponent resolveSimulationComponent = ref entity.Get<ResolveSimulationComponent>();

			ref Position2DComponent positionComponent = ref entity.Get<Position2DComponent>();


			GameObject source = resolveSimulationComponent.Source as GameObject;

			if (source == null)
				return;


			var worldPosition = source.transform.position;


			TransformPosition2DViewComponent positionViewComponent = source.GetComponentInChildren<TransformPosition2DViewComponent>();

			if (positionViewComponent != null)
			{
				worldPosition = positionViewComponent.PositionTransform.position;
			}

			positionComponent.Position = MathHelpersUnity.Vector3To2XZ(worldPosition);

			if (entity.Has<Transform2DComponent>())
			{
				ref Transform2DComponent transformComponent = ref entity.Get<Transform2DComponent>();

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
