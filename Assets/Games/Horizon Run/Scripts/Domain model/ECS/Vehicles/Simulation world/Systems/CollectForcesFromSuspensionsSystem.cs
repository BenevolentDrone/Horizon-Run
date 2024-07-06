using System;

using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class CollectForcesFromSuspensionsSystem : AEntitySetSystem<float>
	{
		private readonly HorizonRunEntityManager entityManager;

		public CollectForcesFromSuspensionsSystem(
			World world,
			HorizonRunEntityManager entityManager)
			: base(
				world
					.GetEntities()
					.With<FourWheeledVehicleComponent>()
					.With<Position3DComponent>()
					.With<Transform3DComponent>()
					.With<PhysicsBody3DComponent>()
					.AsSet())
		{
			this.entityManager = entityManager;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var positionComponent = entity.Get<Position3DComponent>();

			var transformComponent = entity.Get<Transform3DComponent>();


			ref var fourWheeledVehicleComponent = ref entity.Get<FourWheeledVehicleComponent>();

			ref var physicsBody3DComponent = ref entity.Get<PhysicsBody3DComponent>();

			Vector3 pushForce = Vector3.zero;


			for (int i = 0; i < 4; i++)
			{
				Guid wheelEntityID = default;

				switch (i)
				{
					case 0:
						wheelEntityID = fourWheeledVehicleComponent.FrontRightWheel;
						break;

					case 1:
						wheelEntityID = fourWheeledVehicleComponent.RearRightWheel;
						break;

					case 2:
						wheelEntityID = fourWheeledVehicleComponent.RearLeftWheel;
						break;

					case 3:
						wheelEntityID = fourWheeledVehicleComponent.FrontLeftWheel;
						break;
				}

				if (!entityManager.HasEntity(wheelEntityID))
					continue;

				var wheelEntity = entityManager.GetEntity(
					wheelEntityID,
					WorldConstants.SIMULATION_WORLD_ID);

				if (!wheelEntity.IsAlive)
					continue;

				if (!wheelEntity.Has<Suspension3DComponent>())
					continue;

				var suspensionComponent = wheelEntity.Get<Suspension3DComponent>();

				if (suspensionComponent.SuspensionForceOnJoint.magnitude < MathHelpers.EPSILON
					&& suspensionComponent.SuspensionConstraintForceOnJoint.magnitude < MathHelpers.EPSILON)
				{
					continue;
				}

				Vector3 springJointWorldPosition = TransformHelpers.GetWorldPosition3D(
					suspensionComponent.JointPosition,
					transformComponent.TRSMatrix);

				PhysicsHelpers.ApplyForceAt(
					suspensionComponent.SuspensionForceOnJoint,
					springJointWorldPosition,
					positionComponent.Position,
					ref physicsBody3DComponent);

				pushForce += suspensionComponent.SuspensionConstraintForceOnJoint;
			}

			PhysicsHelpers.AddConstraintForce(
				pushForce,
				ref physicsBody3DComponent);
		}
	}
}