using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
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

			var positionComponent = targetEntity.Get<Position3DComponent>();

			var quaternionComponent = targetEntity.Get<QuaternionComponent>();


			wheel3DDebugViewComponent.WheelPosition = positionComponent.Position;

			wheel3DDebugViewComponent.WheelRotation = quaternionComponent.Quaternion;

			wheel3DDebugViewComponent.WheelRadius = wheelComponent.Radius;

			wheel3DDebugViewComponent.WheelWidth = wheelComponent.Width;
		}
	}
}