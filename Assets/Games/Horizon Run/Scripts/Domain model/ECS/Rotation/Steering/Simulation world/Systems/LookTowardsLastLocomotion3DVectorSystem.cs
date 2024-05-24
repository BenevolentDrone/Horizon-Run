using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class LookTowardsLastLocomotion3DVectorSystem : AEntitySetSystem<float>
	{
		public LookTowardsLastLocomotion3DVectorSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<QuaternionSteeringComponent>()
					.With<Locomotion3DComponent>()
					.With<Locomotion3DMemoryComponent>()
					.Without<Transform3DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var steeringComponent = ref entity.Get<QuaternionSteeringComponent>();

			var locomotion3DComponent = entity.Get<Locomotion3DComponent>();

			if (locomotion3DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
				return;

			var locomotionMemoryComponent = entity.Get<Locomotion3DMemoryComponent>();


			var lastLocomotionVectorNormalized = locomotionMemoryComponent.LastLocomotionVectorNormalized;

			Quaternion targetQuaternion = Quaternion.LookRotation(
				lastLocomotionVectorNormalized,
				Vector3.up);

			steeringComponent.TargetQuaternion = targetQuaternion;
		}
	}
}