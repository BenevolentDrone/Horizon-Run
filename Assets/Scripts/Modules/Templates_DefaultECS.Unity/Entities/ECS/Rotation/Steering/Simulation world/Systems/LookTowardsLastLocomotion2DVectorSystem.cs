using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class LookTowardsLastLocomotion2DVectorSystem : AEntitySetSystem<float>
	{
		public LookTowardsLastLocomotion2DVectorSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<UniformSteeringComponent>()
					.With<Locomotion2DComponent>()
					.With<Locomotion2DMemoryComponent>()
					.Without<Transform2DComponent>()
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
		}
	}
}