using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class UniformSteeringSystem : AEntitySetSystem<float>
	{
		public UniformSteeringSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<UniformRotationComponent>()
					.With<UniformSteeringComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var rotationComponent = ref entity.Get<UniformRotationComponent>();

			var steeringComponent = entity.Get<UniformSteeringComponent>();

			float fullAngularDistance = Mathf.Abs(
				Mathf.DeltaAngle(
					rotationComponent.Angle,
					steeringComponent.TargetAngle));

			if (fullAngularDistance < MathHelpers.EPSILON)
				return;
			
			float newAngle = rotationComponent.Angle;

			newAngle =
				Mathf.LerpAngle(
					rotationComponent.Angle,
					steeringComponent.TargetAngle,
					Mathf.Clamp01(
						steeringComponent.AngularSpeed * deltaTime / fullAngularDistance));
				
			rotationComponent.Angle = newAngle.SanitizeAngle();
		}
	}
}