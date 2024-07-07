using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class Transform2DUpdateSystem : AEntitySetSystem<float>
	{
		private readonly DefaultECSEntityListManager entityListManager;

		public Transform2DUpdateSystem(
			World world,
			DefaultECSEntityListManager entityListManager)
			: base(
				world
					.GetEntities()
					.With<Transform2DComponent>()
					.AsSet())
		{
			this.entityListManager = entityListManager;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformComponent = ref entity.Get<Transform2DComponent>();

			if (!transformComponent.Dirty)
			{
				return;
			}

			TransformHelpers.UpdateTransform2DRecursively(
				entity,
				entityListManager);
		}
	}
}