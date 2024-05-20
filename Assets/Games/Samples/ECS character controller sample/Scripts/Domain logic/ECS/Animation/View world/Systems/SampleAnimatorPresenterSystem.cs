using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleAnimatorPresenterSystem : AEntitySetSystem<float>
	{
		public SampleAnimatorPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleAnimatorPresenterComponent>()
					.With<SampleAnimatorViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleAnimatorPresenterComponent = ref entity.Get<SampleAnimatorPresenterComponent>();

			ref var sampleAnimatorViewComponent = ref entity.Get<SampleAnimatorViewComponent>();


			var targetEntity = sampleAnimatorPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}

			ref var sampleLocomotionComponent = ref targetEntity.Get<SampleLocomotionComponent>();

			sampleAnimatorViewComponent.LocomotionVector = sampleLocomotionComponent.LocomotionVector;
		}
	}
}