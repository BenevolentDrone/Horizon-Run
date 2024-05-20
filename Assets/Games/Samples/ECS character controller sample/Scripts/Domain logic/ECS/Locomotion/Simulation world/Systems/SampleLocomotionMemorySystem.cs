using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleLocomotionMemorySystem : AEntitySetSystem<float>
	{
		public SampleLocomotionMemorySystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleLocomotionComponent>()
					.With<SampleLocomotionMemoryComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleLocomotionComponent = ref entity.Get<SampleLocomotionComponent>();

			ref var sampleLocomotionMemoryComponent = ref entity.Get<SampleLocomotionMemoryComponent>();


			if (sampleLocomotionComponent.LocomotionVector.magnitude > MathHelpers.EPSILON)
			{
				sampleLocomotionMemoryComponent.LastLocomotionVector = sampleLocomotionComponent.LocomotionVector;
			}
		}
	}
}