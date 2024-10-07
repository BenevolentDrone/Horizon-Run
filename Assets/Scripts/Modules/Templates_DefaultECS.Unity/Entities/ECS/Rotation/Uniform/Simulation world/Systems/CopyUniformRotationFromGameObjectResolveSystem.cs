using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class CopyUniformRotationFromGameObjectResolveSystem<TSceneEntity>
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

			if (!entity.Has<UniformRotationComponent>())
				return;


			ref ResolveComponent ResolveComponent = ref entity.Get<ResolveComponent>();

			ref UniformRotationComponent uniformRotationComponent = ref entity.Get<UniformRotationComponent>();


			GameObject source = ResolveComponent.Source as GameObject;

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
