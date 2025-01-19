using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class Transform2DUpdateSystem : AEntitySetSystem<float>
	{
		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public Transform2DUpdateSystem(
			World world,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger)
			: base(
				world
					.GetEntities()
					.With<Transform2DComponent>()
					.AsSet())
		{
			this.entityHierarchyManager = entityHierarchyManager;

			this.logger = logger;
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
				entityHierarchyManager,
				logger);
		}
	}
	*/
}