using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Locomotion3DWithTransformSystem : AEntitySetSystem<float>
	{
		public Locomotion3DWithTransformSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Position3DComponent>()
					.With<Locomotion3DComponent>()
					.With<Transform3DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var positionComponent = ref entity.Get<Position3DComponent>();

			var locomotion3DComponent = entity.Get<Locomotion3DComponent>();

			if (locomotion3DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
			{
				return;
			}

			float speed = locomotion3DComponent.LocomotionSpeedNormal * locomotion3DComponent.MaxLocomotionSpeed;

			Vector3 locomotionVector =
				locomotion3DComponent.LocomotionVectorNormalized
				* (speed * deltaTime);

			positionComponent.Position += locomotionVector;

			ref var transformComponent = ref entity.Get<Transform3DComponent>();

			transformComponent.Dirty = true;
		}
	}
}