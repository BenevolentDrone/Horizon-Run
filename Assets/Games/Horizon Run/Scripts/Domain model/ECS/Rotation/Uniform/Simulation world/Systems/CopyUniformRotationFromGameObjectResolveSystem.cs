using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class CopyUniformRotationFromGameObjectResolveSystem<TSceneEntity>
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

			if (!entity.Has<UniformRotationComponent>())
				return;


			ref ResolveSimulationComponent resolveSimulationComponent = ref entity.Get<ResolveSimulationComponent>();

			ref UniformRotationComponent uniformRotationComponent = ref entity.Get<UniformRotationComponent>();


			GameObject source = resolveSimulationComponent.Source as GameObject;

			if (source == null)
				return;


			TransformUniformRotationViewComponent rotationViewComponent = source.GetComponentInChildren<TransformUniformRotationViewComponent>();

			if (rotationViewComponent == null)
				return;


			var angle = rotationViewComponent.RotationPivotTransform.eulerAngles;

			uniformRotationComponent.Angle = MathHelpers.SanitizeAngle(angle.y);
		}

		/// <summary>
		/// Disposes the system.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
