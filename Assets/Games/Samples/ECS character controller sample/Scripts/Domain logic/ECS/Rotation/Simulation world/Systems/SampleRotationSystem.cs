using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleRotationSystem : AEntitySetSystem<float>
	{
		public SampleRotationSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleRotationComponent>()
					.With<SampleSteeringComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleRotationComponent = ref entity.Get<SampleRotationComponent>();

			ref var sampleSteeringComponent = ref entity.Get<SampleSteeringComponent>();

			float fullAngularDistance = Mathf.Abs(
				Mathf.DeltaAngle(
					sampleRotationComponent.Angle,
					sampleSteeringComponent.TargetAngle));

			float newAngle = sampleRotationComponent.Angle;

			if (fullAngularDistance > MathHelpers.EPSILON)
			{
				newAngle =
					Mathf.LerpAngle(
						sampleRotationComponent.Angle,
						sampleSteeringComponent.TargetAngle,
						Mathf.Clamp01(
							sampleSteeringComponent.AngularSpeed * deltaTime / fullAngularDistance));
			}

			sampleRotationComponent.Angle = newAngle;
		}
	}
}