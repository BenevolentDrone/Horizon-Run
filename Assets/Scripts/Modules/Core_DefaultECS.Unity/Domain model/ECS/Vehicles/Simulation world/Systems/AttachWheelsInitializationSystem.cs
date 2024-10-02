using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class AttachWheelsInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly World worldForOverrides;

		private readonly UniversalTemplateEntityManager entityManager;

		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		public AttachWheelsInitializationSystem(
			World worldForOverrides,
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager)
		{
			this.worldForOverrides = worldForOverrides;

			this.entityManager = entityManager;

			this.entityHierarchyManager = entityHierarchyManager;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<FourWheeledVehicleComponent>())
				return;

			if (!entity.Has<AttachWheelsComponent>())
				return;


			Vector3 vehiclePosition = Vector3.zero;

			Quaternion vehicleRotation = Quaternion.identity;

			Vector3 vehicleScale = Vector3.one;

			if (entity.Has<Position3DComponent>())
			{
				var position3DComponent = entity.Get<Position3DComponent>();

				vehiclePosition = position3DComponent.Position;
			}

			if (entity.Has<QuaternionComponent>())
			{
				var quaternionComponent = entity.Get<QuaternionComponent>();

				vehicleRotation = quaternionComponent.Quaternion;
			}

			Matrix4x4 vehicleTRSMatrix = Matrix4x4.TRS(
				vehiclePosition,
				vehicleRotation,
				vehicleScale);

			var attachWheelsComponent = entity.Get<AttachWheelsComponent>();

			ref var fourWheeledVehicleComponent = ref entity.Get<FourWheeledVehicleComponent>();

			for (int i = 0; i < 4; i++)
			{
				Vector3 wheelJointLocalPosition = Vector3.zero;

				Quaternion wheelJointLocalRotation = Quaternion.identity;

				Vector3 wheelJointLocalScale = Vector3.one;

				switch (i)
				{
					case 0:
						wheelJointLocalPosition = fourWheeledVehicleComponent.FrontRightWheelJointPosition;
						break;
					case 1:
						wheelJointLocalPosition = fourWheeledVehicleComponent.RearRightWheelJointPosition;
						break;
					case 2:
						wheelJointLocalPosition = fourWheeledVehicleComponent.RearLeftWheelJointPosition;
						break;
					case 3:
						wheelJointLocalPosition = fourWheeledVehicleComponent.FrontLeftWheelJointPosition;
						break;
				}

				Matrix4x4 wheelJointTRSMatrix = vehicleTRSMatrix * Matrix4x4.TRS(
					wheelJointLocalPosition,
					wheelJointLocalRotation,
					wheelJointLocalScale);

				Vector3 wheelJointWorldPosition = wheelJointTRSMatrix.GetColumn(3);

				Entity @wheelOverride = worldForOverrides.CreateEntity();

				@wheelOverride.Set<Position3DComponent>(
					new Position3DComponent()
					{
						Position = wheelJointLocalPosition
					});

				Vector3 suspensionDirectionNormalized = Vector3.down;

				LocalTransform3D jointPosition = new LocalTransform3D()
				{
					LocalPosition = wheelJointLocalPosition
						- suspensionDirectionNormalized
							* (attachWheelsComponent.DesiredSuspensionRestLength - attachWheelsComponent.DesiredSuspensionTravelLength),
					LocalRotation = Quaternion.identity,
					LocalScale = Vector3.one
				};

				@wheelOverride.Set<Suspension3DComponent>(
					new Suspension3DComponent()
					{
						JointPosition = jointPosition,
						SuspensionDirectionNormalized = suspensionDirectionNormalized,
						RestLength = attachWheelsComponent.DesiredSuspensionRestLength,
						TravelLength = attachWheelsComponent.DesiredSuspensionTravelLength,
						Stiffness = attachWheelsComponent.DesiredSuspensionStiffness,
						Damping = attachWheelsComponent.DesiredSuspensionDamping,
						SuspensionAttachmentReceivesForce = true,
						SuspensionJointReceivesForce = false
					});

				var wheelEntityID = entityManager.SpawnEntity(
					attachWheelsComponent.WheelPrototypeID,
					new[]
					{
						new WorldOverrideDescriptor<Entity>()
						{
							WorldID = WorldConstants.SIMULATION_WORLD_ID,

							OverrideEntity = @wheelOverride
						}
					});

				switch (i)
				{
					case 0:
						fourWheeledVehicleComponent.FrontRightWheel = wheelEntityID;
						break;
					case 1:
						fourWheeledVehicleComponent.RearRightWheel = wheelEntityID;
						break;
					case 2:
						fourWheeledVehicleComponent.RearLeftWheel = wheelEntityID;
						break;
					case 3:
						fourWheeledVehicleComponent.FrontLeftWheel = wheelEntityID;
						break;
				}

				EntityHierarchyHelpers.AddChild(
					entity,
					entityManager.GetEntity(
						wheelEntityID,
						WorldConstants.SIMULATION_WORLD_ID),
						entityHierarchyManager);
			}

			entity.Remove<AttachWheelsComponent>();
		}

		public void Dispose()
		{
		}
	}
}
