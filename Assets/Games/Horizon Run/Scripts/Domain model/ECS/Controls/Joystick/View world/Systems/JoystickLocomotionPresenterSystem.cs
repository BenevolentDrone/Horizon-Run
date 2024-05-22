using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class JoystickLocomotionPresenterSystem : AEntitySetSystem<float>
	{
		public JoystickLocomotionPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<JoystickPresenterComponent>()
					.With<JoystickViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var joystickPresenterComponent = entity.Get<JoystickPresenterComponent>();

			var joystickViewComponent = entity.Get<JoystickViewComponent>();


			var targetEntity = joystickPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			if (targetEntity.Has<Locomotion2DComponent>())
			{
				ref var locomotion2DComponent = ref targetEntity.Get<Locomotion2DComponent>();

				locomotion2DComponent.LocomotionVectorNormalized = joystickViewComponent.Joystick.Direction.normalized;

				locomotion2DComponent.LocomotionSpeedNormal = (joystickViewComponent.Joystick.Direction.magnitude > MathHelpers.EPSILON)
					? 1f
					: 0f;
			}

			if (targetEntity.Has<Locomotion3DComponent>())
			{
				ref var locomotion3DComponent = ref targetEntity.Get<Locomotion3DComponent>();

				Vector2 directionNormalized = joystickViewComponent.Joystick.Direction.normalized;

				Vector3 locomotionVector = MathHelpersUnity.Vector2XZTo3(directionNormalized);

				locomotion3DComponent.LocomotionVectorNormalized = locomotionVector;

				locomotion3DComponent.LocomotionSpeedNormal = (joystickViewComponent.Joystick.Direction.magnitude > MathHelpers.EPSILON)
					? 1f
					: 0f;
			}
		}
	}
}