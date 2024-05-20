using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleTransformRotationViewSystem : AEntitySetSystem<float>
	{
		public SampleTransformRotationViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleTransformRotationViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleTransformRotationViewComponent = ref entity.Get<SampleTransformRotationViewComponent>();

			if (!sampleTransformRotationViewComponent.Dirty)
			{
				return;
			}

			sampleTransformRotationViewComponent.RotationPivotTransform.eulerAngles =
				MathHelpersUnity.AxisVector3Y(sampleTransformRotationViewComponent.Angle);

			sampleTransformRotationViewComponent.Dirty = false;
		}
	}
}