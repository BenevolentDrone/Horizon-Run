using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class Transform3DUpdateSystem : AEntitySetSystem<float>
	{
		private readonly DefaultECSEntityListManager entityListManager;

		public Transform3DUpdateSystem(
			World world,
			DefaultECSEntityListManager entityListManager)
			: base(
				world
					.GetEntities()
					.With<Transform3DComponent>()
					.AsSet())
		{
			this.entityListManager = entityListManager;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformComponent = ref entity.Get<Transform3DComponent>();

			if (!transformComponent.Dirty)
			{
				return;
			}

			TransformHelpers.UpdateTransform3DRecursively(
				entity,
				entityListManager);
		}
	}
}