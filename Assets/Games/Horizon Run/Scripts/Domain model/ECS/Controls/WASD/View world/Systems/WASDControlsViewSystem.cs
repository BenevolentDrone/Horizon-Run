using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class WASDControlsViewSystem : AEntitySetSystem<float>
	{
		public WASDControlsViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<WASDControlsViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var WASDControlsViewComponent = ref entity.Get<WASDControlsViewComponent>();

			Vector2 Direction = Vector2.zero;

			if (Input.GetKey(KeyCode.W))
				Direction += Vector2.up;

			if (Input.GetKey(KeyCode.A))
				Direction += Vector2.left;

			if (Input.GetKey(KeyCode.S))
				Direction += Vector2.down;

			if (Input.GetKey(KeyCode.D))
				Direction += Vector2.right;

			WASDControlsViewComponent.Direction = Direction;
		}
	}
}