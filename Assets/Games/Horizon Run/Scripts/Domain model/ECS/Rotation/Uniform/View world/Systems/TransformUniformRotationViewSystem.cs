using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	public class TransformUniformRotationViewSystem : AEntitySetSystem<float>
	{
		public TransformUniformRotationViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<TransformUniformRotationViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformRotationViewComponent = ref entity.Get<TransformUniformRotationViewComponent>();

			if (!transformRotationViewComponent.Dirty)
			{
				return;
			}

			transformRotationViewComponent.RotationPivotTransform.eulerAngles = new Vector3(
				0f,
				transformRotationViewComponent.Angle,
				0f);

			transformRotationViewComponent.Dirty = false;
		}
	}
}