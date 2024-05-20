using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleLocomotionSystem : AEntitySetSystem<float>
	{
		public SampleLocomotionSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SamplePositionComponent>()
					.With<SampleLocomotionComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var samplePositionComponent = ref entity.Get<SamplePositionComponent>();

			ref var sampleLocomotionComponent = ref entity.Get<SampleLocomotionComponent>();


			if (sampleLocomotionComponent.LocomotionVector.magnitude < MathHelpers.EPSILON)
			{
				return;
			}

			Vector2 locomotionVector = 
				sampleLocomotionComponent.LocomotionVector.normalized
				* (sampleLocomotionComponent.MaxLocomotionSpeed * deltaTime);

			samplePositionComponent.Position += locomotionVector;
		}
	}
}