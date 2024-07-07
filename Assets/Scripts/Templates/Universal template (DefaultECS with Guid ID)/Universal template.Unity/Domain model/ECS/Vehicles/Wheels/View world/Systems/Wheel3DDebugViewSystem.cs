using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class Wheel3DDebugViewSystem : AEntitySetSystem<float>
	{
		public Wheel3DDebugViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Wheel3DDebugViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var wheel3DDebugViewComponent = entity.Get<Wheel3DDebugViewComponent>();

			Vector3 wheelRight = wheel3DDebugViewComponent.WheelRotation * Vector3.right;

			Vector3 wheelHalfWidthPadding = wheelRight * (wheel3DDebugViewComponent.WheelWidth * 0.5f);

			UnityDebugHelpers.DrawCircle3DYZ(
				wheel3DDebugViewComponent.WheelPosition + wheelHalfWidthPadding,
				wheel3DDebugViewComponent.WheelRotation,
				wheel3DDebugViewComponent.WheelRadius,
				32,
				Color.red);

			UnityDebugHelpers.DrawCircle3DYZ(
				wheel3DDebugViewComponent.WheelPosition - wheelHalfWidthPadding,
				wheel3DDebugViewComponent.WheelRotation,
				wheel3DDebugViewComponent.WheelRadius,
				32,
				Color.red);

			//Gizmos.DrawWireSphere(
			//	wheel3DDebugViewComponent.WheelPosition,
			//	wheel3DDebugViewComponent.WheelRadius);
		}
	}
}