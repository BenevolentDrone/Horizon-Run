using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleTransformPositionViewSystem : AEntitySetSystem<float>
	{
		public SampleTransformPositionViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleTransformPositionViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleTransformPositionViewComponent = ref entity.Get<SampleTransformPositionViewComponent>();

			if (!sampleTransformPositionViewComponent.Dirty)
			{
				return;
			}

			sampleTransformPositionViewComponent.PositionTransform.position =
				MathHelpersUnity.Vector2XZTo3(sampleTransformPositionViewComponent.Position);

			sampleTransformPositionViewComponent.Dirty = false;
		}
	}
}