using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class AttachWheelsInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly World worldForOverrides;

		private readonly HorizonRunEntityManager entityManager;

		private readonly DefaultECSEntityListManager entityListManager;

		public AttachWheelsInitializationSystem(
			World worldForOverrides,
			HorizonRunEntityManager entityManager,
			DefaultECSEntityListManager entityListManager)
		{
			this.worldForOverrides = worldForOverrides;

			this.entityManager = entityManager;

			this.entityListManager = entityListManager;
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

			var attachWheelsComponent = entity.Get<AttachWheelsComponent>();

			ref var fourWheeledVehicleComponent = ref entity.Get<FourWheeledVehicleComponent>();

			for (int i = 0; i < 4; i++)
			{
				Vector3 wheelJointPosition = Vector3.zero;

				switch (i)
				{
					case 0:
						wheelJointPosition = fourWheeledVehicleComponent.FrontRightWheelJointPosition;
						break;
					case 1:
						wheelJointPosition = fourWheeledVehicleComponent.RearRightWheelJointPosition;
						break;
					case 2:
						wheelJointPosition = fourWheeledVehicleComponent.RearLeftWheelJointPosition;
						break;
					case 3:
						wheelJointPosition = fourWheeledVehicleComponent.FrontLeftWheelJointPosition;
						break;
				}

				Entity @wheelOverride = worldForOverrides.CreateEntity();

				@wheelOverride.Set<Position3DComponent>(
					new Position3DComponent()
					{
						Position = wheelJointPosition
					});

				Vector3 suspensionDirectionNormalized = Vector3.down;

				LocalTransform3D jointPosition = new LocalTransform3D()
				{
					LocalPosition = wheelJointPosition
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
						Damping = attachWheelsComponent.DesiredSuspensionDamping
					});

				var wheelEntityID = entityManager.SpawnEntity(
					attachWheelsComponent.WheelPrototypeID,
					new[]
					{
						new PrototypeOverride<Entity>()
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

				HierarchyHelpers.AddChild(
					entity,
					entityManager.GetEntity(
						wheelEntityID,
						WorldConstants.SIMULATION_WORLD_ID),
						entityListManager);
			}

			entity.Remove<AttachWheelsComponent>();
		}

		public void Dispose()
		{
		}
	}
}
