using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class WASDControlsLocomotionPresenterSystem : AEntitySetSystem<float>
	{
		public WASDControlsLocomotionPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<WASDControlsPresenterComponent>()
					.With<WASDControlsViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var wasdControlsPresenterComponent = entity.Get<WASDControlsPresenterComponent>();

			var wasdControlsViewComponent = entity.Get<WASDControlsViewComponent>();


			var targetEntity = wasdControlsPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			if (targetEntity.Has<Locomotion2DComponent>())
			{
				ref var locomotion2DComponent = ref targetEntity.Get<Locomotion2DComponent>();

				locomotion2DComponent.LocomotionVectorNormalized = wasdControlsViewComponent.Direction.normalized;

				locomotion2DComponent.LocomotionSpeedNormal = (wasdControlsViewComponent.Direction.magnitude > MathHelpers.EPSILON)
					? 1f
					: 0f;
			}

			if (targetEntity.Has<Locomotion3DComponent>())
			{
				ref var locomotion3DComponent = ref targetEntity.Get<Locomotion3DComponent>();

				Vector2 directionNormalized = wasdControlsViewComponent.Direction.normalized;

				Vector3 locomotionVector = MathHelpersUnity.Vector2XZTo3(directionNormalized);

				locomotion3DComponent.LocomotionVectorNormalized = locomotionVector;

				locomotion3DComponent.LocomotionSpeedNormal = (wasdControlsViewComponent.Direction.magnitude > MathHelpers.EPSILON)
					? 1f
					: 0f;
			}
		}
	}
}