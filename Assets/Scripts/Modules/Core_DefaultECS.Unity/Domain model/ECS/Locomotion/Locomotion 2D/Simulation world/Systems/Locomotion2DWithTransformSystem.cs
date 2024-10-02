using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class Locomotion2DWithTransformSystem : AEntitySetSystem<float>
	{
		public Locomotion2DWithTransformSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Position2DComponent>()
					.With<Locomotion2DComponent>()
					.With<Transform2DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var position2DComponent = ref entity.Get<Position2DComponent>();

			var locomotion2DComponent = entity.Get<Locomotion2DComponent>();


			if (locomotion2DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
			{
				return;
			}

			float speed = locomotion2DComponent.LocomotionSpeedNormal * locomotion2DComponent.MaxLocomotionSpeed;

			Vector2 locomotionVector =
				locomotion2DComponent.LocomotionVectorNormalized
				* (speed * deltaTime);

			position2DComponent.Position += locomotionVector;

			ref var transformComponent = ref entity.Get<Transform2DComponent>();

			transformComponent.Dirty = true;
		}
	}
}