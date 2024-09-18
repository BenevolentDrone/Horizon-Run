using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class Transform3DUpdateSystem : AEntitySetSystem<float>
	{
		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public Transform3DUpdateSystem(
			World world,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger = null)
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

				/*
				UnityEngine.Debug.DrawLine(
					worldPosition,
					worldPosition + worldRotation * (Vector3.right * (1f / 3f)),
					Color.red);

				UnityEngine.Debug.DrawLine(
					worldPosition,
					worldPosition + worldRotation * (Vector3.up * (1f / 3f)),
					Color.green);

				UnityEngine.Debug.DrawLine(
					worldPosition,
					worldPosition + worldRotation * (Vector3.forward * (1f / 3f)),
					Color.blue);
				*/

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

			/*
			UnityEngine.Debug.DrawLine(
				worldPosition,
				worldPosition + worldRotation * (Vector3.right * (1f / 3f)),
				Color.red);

			UnityEngine.Debug.DrawLine(
				worldPosition,
				worldPosition + worldRotation * (Vector3.up * (1f / 3f)),
				Color.green);

			UnityEngine.Debug.DrawLine(
				worldPosition,
				worldPosition + worldRotation * (Vector3.forward * (1f / 3f)),
				Color.blue);
			*/
		}
	}
}