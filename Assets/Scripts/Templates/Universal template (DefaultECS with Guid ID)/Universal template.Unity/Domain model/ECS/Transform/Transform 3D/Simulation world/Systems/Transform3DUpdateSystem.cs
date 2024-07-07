using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
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
				entityListManager);

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