using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;
using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Time;

using HereticalSolutions.Entities;

using HereticalSolutions.Templates.Universal;
using HereticalSolutions.Templates.Universal.Unity;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun.DI
{
	public static class SystemsFactory
	{
		#region View world systems

		public static void InitializeViewWorldSystemsContainer(
			IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem> viewWorldSystemsContainer,
			UniversalTemplateEntityManager entityManager,
			IManagedPool<GameObject> gameObjectPool,
			Transform hudCanvasTransform,
			ILoggerResolver loggerResolver = null)
		{
			viewWorldSystemsContainer.Initialize(
				BuildViewWorldResolveSystems(
					gameObjectPool,
					loggerResolver),
				BuildViewWorldInitializationSystems(
					entityManager,
					gameObjectPool,
					hudCanvasTransform,
					loggerResolver),
				BuildViewWorldDeinitializationSystems());
		}

		public static IDefaultECSEntityInitializationSystem BuildViewWorldResolveSystems(
			IManagedPool<GameObject> gameObjectPool,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(
				new ResolvePooledGameObjectViewSystem<UniversalTemplateSceneEntity>(
					gameObjectPool,
					loggerResolver?.GetLogger<ResolvePooledGameObjectViewSystem<UniversalTemplateSceneEntity>>())
				);
		}

		public static IDefaultECSEntityInitializationSystem BuildViewWorldInitializationSystems(
			UniversalTemplateEntityManager entityManager,
			IManagedPool<GameObject> gameObjectPool,
			Transform hudCanvasTransform,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Pooled game object views
				new SpawnPooledGameObjectViewSystem(
					gameObjectPool,
					loggerResolver?.GetLogger<SpawnPooledGameObjectViewSystem>()),
	
				BuildPresenterInitializationSystems(
					entityManager,
					loggerResolver),

				new	AttachToHUDCanvasInitializationSystem(
					hudCanvasTransform,
					loggerResolver?.GetLogger<AttachToHUDCanvasInitializationSystem>()));
		}

		private static ISystem<Entity> BuildPresenterInitializationSystems(
			UniversalTemplateEntityManager entityManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(
				// Position presenters initialization
				new Position2DPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<Position2DPresenterInitializationSystem>()),
				new Position3DPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<Position3DPresenterInitializationSystem>()),

				// Rotation presenters initialization
				new UniformRotationPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<UniformRotationPresenterInitializationSystem>()),
				new QuaternionPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<QuaternionPresenterInitializationSystem>()),

				// Suspension presenters initialization
				new Suspension3DPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<Suspension3DPresenterInitializationSystem>()),

				// Wheel presenters initialization
				new Wheel3DPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<Wheel3DPresenterInitializationSystem>())
				);
		}

		public static IDefaultECSEntityInitializationSystem BuildViewWorldDeinitializationSystems(
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(
				new PushPooledGameObjectViewSystem(
					loggerResolver?.GetLogger<PushPooledGameObjectViewSystem>()));
		}

		#endregion

		#region Simulation world systems

		public static void InitializeSimulationWorldSystemsContainer(
			World worldForOverrides,
			IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem> simulationWorldSystemsContainer,
			UniversalTemplateEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			simulationWorldSystemsContainer.Initialize(
				BuildSimulationWorldResolveSystems(
					entityManager,
					entityHierarchyManager,
					loggerResolver),
				BuildSimulationWorldInitializationSystems(
					worldForOverrides,
					entityManager,
					eventEntityBuilder,
					entityListManager,
					entityHierarchyManager,
					simulationUpdateTimerManager,
					loggerResolver),
				BuildSimulationWorldDeinitializationSystems(
					entityManager,
					entityListManager,
					entityHierarchyManager,
					simulationUpdateTimerManager,
					loggerResolver));
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldResolveSystems(
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Position
				new CopyPosition2DFromGameObjectResolveSystem<UniversalTemplateSceneEntity>(),
				new CopyPosition3DFromGameObjectResolveSystem<UniversalTemplateSceneEntity>(),

				// Rotation
				new CopyUniformRotationFromGameObjectResolveSystem<UniversalTemplateSceneEntity>(),
				new CopyQuaternionFromGameObjectResolveSystem<UniversalTemplateSceneEntity>(),

				// Sub spawners
				new LinkSubSpawnersResolveSystem<UniversalTemplateSceneEntity>(
					entityManager,
					entityHierarchyManager,
					loggerResolver?.GetLogger<LinkSubSpawnersResolveSystem<UniversalTemplateSceneEntity>>()));
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldInitializationSystems(
			World worldForOverrides,
			UniversalTemplateEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Timers
				new LoopedTimerInitializationSystem(),

				// Hierarchy
				new HierarchyInitializationSystem(
					entityHierarchyManager),

				// Spawners
				new SpawnerWithLoopedTimerInitializationSystem(
					simulationUpdateTimerManager,
					eventEntityBuilder,
					loggerResolver,
					loggerResolver?.GetLogger<SpawnerWithLoopedTimerInitializationSystem>()),
				new EmitImmediatelyInitializationSystem(
					eventEntityBuilder),

				// Sensors
				new SensorWithTimerInitializationSystem(
					simulationUpdateTimerManager,
					eventEntityBuilder,
					loggerResolver,
					loggerResolver?.GetLogger<SensorWithTimerInitializationSystem>()),
				new CreateSensorEntityListInitializationSystem(
					entityListManager),
					
				// Wheels
				new AttachWheelsInitializationSystem(
					worldForOverrides,
					entityManager,
					entityHierarchyManager)
				);
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldDeinitializationSystems(
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityListManager entityListManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Hierarchy
				new HierarchyDeinitializationSystem<GUIDComponent, Guid>(
					entityManager,
					entityHierarchyManager,
					(GUIDComponent) =>
					{
						return GUIDComponent.GUID;
					},
					new PoolWithListCleanup<List<IReadOnlyHierarchyNode<Entity>>>(
						StackPoolFactory.BuildStackPool<List<IReadOnlyHierarchyNode<Entity>>>(
							new AllocationCommand<List<IReadOnlyHierarchyNode<Entity>>>
							{
								Descriptor = new AllocationCommandDescriptor
								{
									Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

									Amount = 5 //TODO: REMOVE MAGIC
								},
								AllocationDelegate = AllocationsFactory.
									ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<Entity>>>
							},
							new AllocationCommand<List<IReadOnlyHierarchyNode<Entity>>>
							{
								Descriptor = new AllocationCommandDescriptor
								{
									Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

									Amount = 5 //TODO: REMOVE MAGIC
								},
								AllocationDelegate = AllocationsFactory.
									ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<Entity>>>
							},
							loggerResolver)),
					loggerResolver?.GetLogger<HierarchyDeinitializationSystem<GUIDComponent, Guid>>()),

				// Sensors
				new RemoveSensorEntityListDeinitializationSystem(
					entityListManager),

				// Looped timer
				new LoopedTimerDeinitializationSystem(
					simulationUpdateTimerManager));
		}

		#endregion

		#region Event world systems

		public static ISystem<float> BuildEventWorldSystems(
			World eventWorld,
			World viewWorld,
			EEntityAuthoringPresets authoringPreset,
			UniversalTemplateEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			bool includeViewWorld,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				// Camera target selected
				(includeViewWorld)
					? BuildCameraTargetSelectedEventSystems(
						eventWorld,
						viewWorld)
					: new PlaceholderSystem(),

				// Input target selected
				(includeViewWorld)
					? BuildInputTargetSelectedEventSystems(
						eventWorld,
						viewWorld)
					: new PlaceholderSystem(),

				// HUD target selected
				(includeViewWorld)
					? BuildHUDTargetSelectedEventSystems(
						eventWorld,
						viewWorld)
					: new PlaceholderSystem(),

				// Spawner emitted
				BuildSpawnerEmittedEventSystems(
					eventWorld,
					eventEntityBuilder,
					loggerResolver),

				// Entity spawned
				BuildEntitySpawnedEventSystems(
					eventWorld,
					authoringPreset,
					entityManager,
					entityHierarchyManager,
					loggerResolver),

				// Sensor scan performed
				BuildSensorScanPerformedEventSystems(
					eventWorld,
					entityListManager,
					loggerResolver),

				BuildEventWorldLifetimeSystems(
					eventWorld));
		}

		private static ISystem<float> BuildCameraTargetSelectedEventSystems(
			World eventWorld,
			World viewWorld)
		{
			return new SequentialSystem<float>(
				new UpdateSelectionOnCameraTargetSelectedEventSystem(
					eventWorld,
					viewWorld),
				new EventProcessedSystem<CameraTargetSelectedEventComponent, float>(
					eventWorld));
		}

		private static ISystem<float> BuildInputTargetSelectedEventSystems(
			World eventWorld,
			World viewWorld)
		{
			return new SequentialSystem<float>(
				new UpdateSelectionOnInputTargetSelectedEventSystem(
					eventWorld,
					viewWorld),
				new EventProcessedSystem<InputTargetSelectedEventComponent, float>(eventWorld));
		}

		private static ISystem<float> BuildHUDTargetSelectedEventSystems(
			World eventWorld,
			World viewWorld)
		{
			return new SequentialSystem<float>(
				new UpdateSelectionOnHUDTargetSelectedEventSystem(
					eventWorld,
					viewWorld),
				new EventProcessedSystem<HUDTargetSelectedEventComponent, float>(eventWorld));
		}

		private static ISystem<float> BuildSpawnerEmittedEventSystems(
			World eventWorld,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				//Checks for parent spawner
				//new CheckEntityPresenceOnSpawnerEmittedEventSystem(
				//	eventWorld,
				//	loggerResolver?.GetLogger<CheckEntityPresenceOnSpawnerEmittedEventSystem>()),

				//Activate child spawners
				new InvokeSubSpawnerOnSpawnerEmittedEventSystem(
					eventWorld,
					eventEntityBuilder),

				new SpawnEntitiesOnSpawnerEmittedEventSystem(
					eventWorld,
					eventEntityBuilder),
				new EventProcessedSystem<SpawnerEmittedEventComponent, float>(eventWorld));
		}

		private static ISystem<float> BuildEntitySpawnedEventSystems(
			World eventWorld,
			EEntityAuthoringPresets authoringPreset,
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(
				new CreateEntityOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager,
					authoringPreset),
				new PlaceEntity2DOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager,
					entityHierarchyManager,
					loggerResolver?.GetLogger<PlaceEntity2DOnEntitySpawnedEventSystem>()),
				new PlaceEntity3DOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager,
					entityHierarchyManager,
					loggerResolver?.GetLogger<PlaceEntity3DOnEntitySpawnedEventSystem>()),
				new RotateEntityOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager),
				new EventProcessedSystem<EntitySpawnedEventComponent, float>(eventWorld));
		}

		private static ISystem<float> BuildSensorScanPerformedEventSystems(
			World eventWorld,
			DefaultECSEntityListManager entityListManager,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(
				new CheckSensorPresenceOnSensorScanPerformedEventSystem(
					eventWorld),
				new ErasePreviousScanResultsOnSensorScanPerformedEventSystem(
					eventWorld,
					entityListManager,
					loggerResolver?.GetLogger<ErasePreviousScanResultsOnSensorScanPerformedEventSystem>()),
				//TODO
				//new PerformRadarSweepOnSensorScanPerformedEventSystem(
				//	eventWorld,
				//	loggerResolver?.GetLogger<PerformRadarSweepOnSensorScanPerformedEventSystem>()),
				new EventProcessedSystem<SensorScanPerformedEventComponent, float>(eventWorld));
		}

		private static ISystem<float> BuildEventWorldLifetimeSystems(
			World eventWorld)
		{
			return new SequentialSystem<float>(
				new DisposeProcessedEventsSystem<float>(eventWorld),

				new DespawnSystem(eventWorld));
		}

		#endregion

		#region Update systems

		public static ISystem<float> BuildUpdateSystems(
			World simulationWorld,
			World viewWorld,
			ITimerManager simulationUpdateTimerManager,
			DefaultECSEntityListManager entityListManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			bool includeViewWorld,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(
				(includeViewWorld)
					? BuildModelValidationSystems(
						viewWorld)
					: new PlaceholderSystem(),

				BuildGameLogicSystems(
					simulationWorld,
					simulationUpdateTimerManager,
					entityHierarchyManager,
					loggerResolver),

				BuildSimulationWorldLifetimeSystems(
					simulationWorld),

				//Otherwise the view world would start with entities pending deletion
				//Also I noticed NO such system in view world
				//TODO: decide whether the view world should have one
				(includeViewWorld)
					? BuildViewWorldLifetimeSystems(
						viewWorld)
					: new PlaceholderSystem()
			);
		}

		private static ISystem<float> BuildModelValidationSystems(
			World viewWorld)
		{
			return new SequentialSystem<float>(

				//Virtual camera
				new VirtualCameraPresenterModelValidationSystem(
					viewWorld),

				//Cursors
				//new CursorSwitcherModelValidationSystem(
				//	viewWorld),

				//Controls
				new JoystickPresenterModelValidationSystem(
					viewWorld),
				new WASDControlsPresenterModelValidationSystem(
					viewWorld));
		}

		private static ISystem<float> BuildGameLogicSystems(
			World simulationWorld,
			ITimerManager simulationUpdateTimerManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				// Transform updating
				new Transform2DUpdateSystem(
					simulationWorld,
					entityHierarchyManager),
				new Transform3DUpdateSystem(
					simulationWorld,
					entityHierarchyManager,
					loggerResolver?.GetLogger<Transform3DUpdateSystem>()),

				// Sensors
				new SensorWithTimerSystem(
					simulationWorld,
					simulationUpdateTimerManager,
					loggerResolver?.GetLogger<SensorWithTimerSystem>())

				//Let's move those higher above so that they get updated at the very start of the frame
				// Transform updating
				//new Transform2DUpdateSystem(
				//	simulationWorld,
				//	entityListManager),
				//new Transform3DUpdateSystem(
				//	simulationWorld,
				//	entityListManager)
				);
		}

		private static ISystem<float> BuildSimulationWorldLifetimeSystems(
			World simulationWorld)
		{
			return new SequentialSystem<float>(

				new DespawnSystem(
					simulationWorld));
		}

		private static ISystem<float> BuildViewWorldLifetimeSystems(
			World viewWorld)
		{
			return new SequentialSystem<float>(

				new DespawnSystem(
					viewWorld));
		}

		#endregion

		#region Fixed update systems

		public static ISystem<float> BuildFixedUpdateSystems(
			World simulationWorld,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

					//Transform updating
					new Transform2DUpdateSystem(
						simulationWorld,
						entityHierarchyManager,
						loggerResolver?.GetLogger<Transform2DUpdateSystem>()),
					new Transform3DUpdateSystem(
						simulationWorld,
						entityHierarchyManager,
						loggerResolver?.GetLogger<Transform3DUpdateSystem>()),

					// Looking
					new LookTowardsLastLocomotion2DVectorSystem(
						simulationWorld),
					new LookTowardsLastLocomotion2DVectorWithTransformSystem(
						simulationWorld),
					new LookTowardsLastLocomotion3DVectorSystem(
						simulationWorld),
					new LookTowardsLastLocomotion3DVectorWithTransformSystem(
						simulationWorld),

					// Steering
					new UniformSteeringSystem(
						simulationWorld),
					new QuaternionSteeringSystem(
						simulationWorld),

					// Locomotion 2D
					new Locomotion2DSystem(
						simulationWorld),
					new Locomotion2DWithTransformSystem(
						simulationWorld),
					new Locomotion2DMemorySystem(
						simulationWorld),

					// Locomotion 3D
					new Locomotion3DSystem(
						simulationWorld),
					new Locomotion3DWithTransformSystem(
						simulationWorld),
					new Locomotion3DMemorySystem(
						simulationWorld),

					// Wheels
					//3 was initial raycast hit count but due to the fucked up Unity's physics module the value
					//and the fact that the wheel spherecast is now taking place from above of it, the value is increased to 5
					//TODO: define mask properly
					new Wheel3DSystem(
						simulationWorld,
						new RaycastHit[5],
						LayerMask.GetMask("Terrain")),

					// Suspension
					new Suspension3DSystem(
						simulationWorld,
						entityHierarchyManager,
						loggerResolver?.GetLogger<Suspension3DSystem>()),

					// Vehicle
					//new CollectForcesFromSuspensionsSystem(
					//	simulationWorld,
					//	entityManager),

					// Physics
					new Gravity3DSystem(
						simulationWorld),
					new PhysicsBody3DSystem(
						simulationWorld)
					);
		}

		#endregion

		#region Late update systems

		public static ISystem<float> BuildLateUpdateSystems(
			World viewWorld,
			UniversalTemplateEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			Camera mainRenderCamera,
			IUIManager uiManager,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				BuildViewInputSystems(
					viewWorld,
					mainRenderCamera,
					uiManager),

				BuildViewPresenterSystems(
					viewWorld,
					entityManager,
					entityHierarchyManager,
					eventEntityBuilder,
					loggerResolver),

				BuildViewVisualsSystems(
					viewWorld));
		}

		private static ISystem<float> BuildViewInputSystems(
			World viewWorld,
			Camera mainRenderCamera,
			IUIManager uiManager)
		{
			return new SequentialSystem<float>(

				//Cursors
				new CursorSwitcherViewSystem(
					viewWorld),
	
				//Controls
				new WASDControlsViewSystem(
					viewWorld),
				new LMBClickViewSystem(
					viewWorld,
					mainRenderCamera,
					uiManager),
				new RMBClickViewSystem(
					viewWorld,
					mainRenderCamera,
					uiManager));
		}

		private static ISystem<float> BuildViewPresenterSystems(
			World viewWorld,
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				// Cursor presenters
				//TODO
				//new CursorSwitcherPresenterSystem(
				//	viewWorld,
				//	entityManager),

				// Controls presenters
				new JoystickLocomotionPresenterSystem(
					viewWorld),
				new WASDControlsLocomotionPresenterSystem(
					viewWorld),
				new LMBClickPresenterSystem(
					viewWorld,
					eventEntityBuilder),
				new RMBClickPresenterSystem(
					viewWorld,
					eventEntityBuilder),

				// Transform view presenters
				new TransformPosition2DPresenterSystem(
					viewWorld),
				new TransformPosition3DPresenterSystem(
					viewWorld),

				// Rotation view presenters
				new UniformRotationPresenterSystem(
					viewWorld),
				new QuaternionPresenterSystem(
					viewWorld),

				// Suspension debug presenters
				new Suspension3DDebugPresenterSystem(
					viewWorld,
					entityHierarchyManager,
					loggerResolver?.GetLogger<Suspension3DDebugPresenterSystem>()),

				// Wheel debug presenters
				new Wheel3DDebugPresenterSystem(
					viewWorld),

				//Camera presenters
				new VirtualCameraPresenterSystem(
					viewWorld));
		}

		private static ISystem<float> BuildViewVisualsSystems(
			World viewWorld)
		{
			return new SequentialSystem<float>(

				// Debug views
				new DebugDrawLineViewSystem(
					viewWorld),

				// Transform position views
				new TransformPosition2DViewSystem(
					viewWorld),
				new TransformPosition3DViewSystem(
					viewWorld),

				// Transform rotation views
				new TransformUniformRotationViewSystem(
					viewWorld),
				new TransformQuaternionViewSystem(
					viewWorld),
					
				// Suspension debug views
				new Suspension3DDebugViewSystem(
					viewWorld),
					
				// Wheel debug views
				new Wheel3DDebugViewSystem(
					viewWorld)
				);
		}

		#endregion
	}
}