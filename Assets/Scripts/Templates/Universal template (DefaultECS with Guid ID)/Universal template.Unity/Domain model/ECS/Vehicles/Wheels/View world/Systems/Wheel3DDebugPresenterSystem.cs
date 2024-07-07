using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class Wheel3DDebugPresenterSystem : AEntitySetSystem<float>
	{
		public Wheel3DDebugPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Wheel3DPresenterComponent>()
					.With<Wheel3DDebugViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var wheel3DPresenterComponent = entity.Get<Wheel3DPresenterComponent>();

			ref var wheel3DDebugViewComponent = ref entity.Get<Wheel3DDebugViewComponent>();


			var targetEntity = wheel3DPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				return;
			}


			var wheelComponent = targetEntity.Get<Wheel3DComponent>();

			var transformComponent = targetEntity.Get<Transform3DComponent>();

			var wheelWorldPosition = TransformHelpers.GetWorldPosition3D(
							transformComponent.TRSMatrix);

			var wheelWorldRotation = transformComponent.TRSMatrix.rotation;


			wheel3DDebugViewComponent.WheelPosition = wheelWorldPosition;

			wheel3DDebugViewComponent.WheelRotation = wheelWorldRotation;

			wheel3DDebugViewComponent.WheelRadius = wheelComponent.Radius;

			wheel3DDebugViewComponent.WheelWidth = wheelComponent.Width;
		}
	}
}