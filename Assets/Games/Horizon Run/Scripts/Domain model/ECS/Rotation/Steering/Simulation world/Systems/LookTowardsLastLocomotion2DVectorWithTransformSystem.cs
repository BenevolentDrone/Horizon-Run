using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class LookTowardsLastLocomotion2DVectorWithTransformSystem : AEntitySetSystem<float>
	{
		public LookTowardsLastLocomotion2DVectorWithTransformSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<UniformSteeringComponent>()
					.With<Locomotion2DComponent>()
					.With<Locomotion2DMemoryComponent>()
					.With<Transform2DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var steeringComponent = ref entity.Get<UniformSteeringComponent>();

			var locomotion2DComponent = entity.Get<Locomotion2DComponent>();

			if (locomotion2DComponent.LocomotionSpeedNormal < MathHelpers.EPSILON)
				return;

			var locomotionMemoryComponent = entity.Get<Locomotion2DMemoryComponent>();


			var lastLocomotionVectorNormalized = locomotionMemoryComponent.LastLocomotionVectorNormalized;

			float angle = MathHelpersUnity.TargetAngle(lastLocomotionVectorNormalized);

			steeringComponent.TargetAngle = angle.SanitizeAngle();

			ref var transformComponent = ref entity.Get<Transform2DComponent>();

			transformComponent.Dirty = true;
		}
	}
}