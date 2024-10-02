using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class Gravity3DSystem : AEntitySetSystem<float>
	{
		public Gravity3DSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Gravity3DComponent>()
					.With<PhysicsBody3DComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var gravity3DComponent = ref entity.Get<Gravity3DComponent>();

			if (!gravity3DComponent.Enabled)
			{
				return;
			}

			ref var physicsBodyComponent = ref entity.Get<PhysicsBody3DComponent>();

			physicsBodyComponent.LinearVelocity += gravity3DComponent.Gravity * deltaTime;
		}
	}
}