using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Locomotion2DSystem : AEntitySetSystem<float>
	{
		public Locomotion2DSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<PositionComponent>()
					.With<Locomotion2DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var positionComponent = ref entity.Get<PositionComponent>();

			var locomotion2DComponent = entity.Get<Locomotion2DComponent>();


			if (locomotion2DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
			{
				return;
			}

			float speed = locomotion2DComponent.LocomotionSpeedNormal * locomotion2DComponent.MaxLocomotionSpeed;

			Vector2 locomotionVector = 
				locomotion2DComponent.LocomotionVectorNormalized
				* (speed * deltaTime);

			positionComponent.Position.x += locomotionVector.x;
			positionComponent.Position.z += locomotionVector.y;
		}
	}
}