using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class UniformRotationPresenterSystem : AEntitySetSystem<float>
	{
		public UniformRotationPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<UniformRotationPresenterComponent>()
					.With<TransformUniformRotationViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var rotationPresenterComponent = entity.Get<UniformRotationPresenterComponent>();

			ref var transformRotationViewComponent = ref entity.Get<TransformUniformRotationViewComponent>();


			var targetEntity = rotationPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			var rotationComponent = targetEntity.Get<UniformRotationComponent>();

			var lastAngle = transformRotationViewComponent.Angle;

			transformRotationViewComponent.Angle = rotationComponent.Angle;

			if (Mathf.Abs(
				Mathf.DeltaAngle(
					lastAngle,
					transformRotationViewComponent.Angle)) > MathHelpers.EPSILON)
				transformRotationViewComponent.Dirty = true;
		}
	}
}