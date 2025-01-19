using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class Transform3DUpdateSystem : AEntitySetSystem<float>
	{
		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public Transform3DUpdateSystem(
			World world,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger)
			: base(
				world
					.GetEntities()
					.With<Transform3DComponent>()
					.AsSet())
		{
			this.entityHierarchyManager = entityHierarchyManager;

			this.logger = logger;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var transformComponent = ref entity.Get<Transform3DComponent>();

			//DEBUG
			Vector3 worldPosition;

			Quaternion worldRotation;

			if (!transformComponent.Dirty)
			{
				//DEBUG
				worldPosition = TransformHelpers.GetWorldPosition3D(
					transformComponent.TRSMatrix);

				worldRotation = transformComponent.TRSMatrix.rotation;

				return;
			}

			TransformHelpers.UpdateTransform3DRecursively(
				entity,
				entityHierarchyManager,
				logger);

			//DEBUG
			worldPosition = TransformHelpers.GetWorldPosition3D(
				transformComponent.TRSMatrix);

			worldRotation = transformComponent.TRSMatrix.rotation;

		}
	}
	*/
}