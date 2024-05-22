using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Locomotion2DMemorySystem : AEntitySetSystem<float>
	{
		public Locomotion2DMemorySystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Locomotion2DComponent>()
					.With<Locomotion2DMemoryComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var locomotion2DComponent = entity.Get<Locomotion2DComponent>();

			ref var locomotionMemoryComponent = ref entity.Get<Locomotion2DMemoryComponent>();


			if (locomotion2DComponent.LocomotionSpeedNormal > MathHelpers.EPSILON)
			{
				locomotionMemoryComponent.LastLocomotionVectorNormalized = locomotion2DComponent.LocomotionVectorNormalized;
			}
		}
	}
}