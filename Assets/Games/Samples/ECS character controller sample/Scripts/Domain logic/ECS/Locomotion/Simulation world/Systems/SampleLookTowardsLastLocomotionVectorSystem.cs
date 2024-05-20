using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleLookTowardsLastLocomotionVectorSystem : AEntitySetSystem<float>
	{
		public SampleLookTowardsLastLocomotionVectorSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleSteeringComponent>()
					.With<SampleLocomotionMemoryComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleSteeringComponent = ref entity.Get<SampleSteeringComponent>();

			ref var sampleLocomotionMemoryComponent = ref entity.Get<SampleLocomotionMemoryComponent>();


			var lastLocomotionVectorNormalized = sampleLocomotionMemoryComponent.LastLocomotionVector.normalized;

			float angle = Mathf.Atan2(
				-lastLocomotionVectorNormalized.y,
				lastLocomotionVectorNormalized.x)
				* Mathf.Rad2Deg
				+ 90f;

			sampleSteeringComponent.TargetAngle = angle;
		}
	}
}