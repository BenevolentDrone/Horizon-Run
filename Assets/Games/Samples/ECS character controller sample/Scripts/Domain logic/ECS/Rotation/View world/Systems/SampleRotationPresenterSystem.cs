using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleRotationPresenterSystem : AEntitySetSystem<float>
	{
		public SampleRotationPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleRotationPresenterComponent>()
					.With<SampleTransformRotationViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleRotationPresenterComponent = ref entity.Get<SampleRotationPresenterComponent>();

			ref var sampleTransformRotationViewComponent = ref entity.Get<SampleTransformRotationViewComponent>();


			var targetEntity = sampleRotationPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			ref var sampleRotationComponent = ref targetEntity.Get<SampleRotationComponent>();

			var lastAngle = sampleTransformRotationViewComponent.Angle;

			sampleTransformRotationViewComponent.Angle = sampleRotationComponent.Angle;

			if (Mathf.Abs(
				Mathf.DeltaAngle(
					lastAngle,
					sampleTransformRotationViewComponent.Angle)) > MathHelpers.EPSILON)
				sampleTransformRotationViewComponent.Dirty = true;
		}
	}
}