using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SamplePositionPresenterSystem : AEntitySetSystem<float>
	{
		public SamplePositionPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SamplePositionPresenterComponent>()
					.With<SampleTransformPositionViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var samplePositionPresenterComponent = ref entity.Get<SamplePositionPresenterComponent>();

			ref var sampleTransformPositionViewComponent = ref entity.Get<SampleTransformPositionViewComponent>();


			var targetEntity = samplePositionPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			ref var samplePositionComponent = ref targetEntity.Get<SamplePositionComponent>();

			var lastPosition = sampleTransformPositionViewComponent.Position;

			sampleTransformPositionViewComponent.Position = samplePositionComponent.Position;

			if ((lastPosition - sampleTransformPositionViewComponent.Position).sqrMagnitude > MathHelpers.EPSILON)
				sampleTransformPositionViewComponent.Dirty = true;
		}
	}
}