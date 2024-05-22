using System;
//using System.Collections.Generic;

using HereticalSolutions.Pools;

//using HereticalSolutions.Repositories;

//using HereticalSolutions.SpacePartitioning;

using HereticalSolutions.Time;

//using HereticalSolutions.Synchronization;

using HereticalSolutions.Entities;

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
			HorizonRunEntityManager entityManager,
			INonAllocDecoratedPool<GameObject> gameObjectPool,
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
			INonAllocDecoratedPool<GameObject> gameObjectPool,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(
				new ResolvePooledGameObjectViewSystem<HorizonRunSceneEntity>(
					gameObjectPool,
					loggerResolver?.GetLogger<ResolvePooledGameObjectViewSystem<HorizonRunSceneEntity>>())
				);
		}

		public static IDefaultECSEntityInitializationSystem BuildViewWorldInitializationSystems(
			HorizonRunEntityManager entityManager,
			INonAllocDecoratedPool<GameObject> gameObjectPool,
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
			HorizonRunEntityManager entityManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(
				new PositionPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<PositionPresenterInitializationSystem>()),

				new UniformRotationPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<UniformRotationPresenterInitializationSystem>()),
				new QuaternionPresenterInitializationSystem(
					entityManager,
					loggerResolver?.GetLogger<QuaternionPresenterInitializationSystem>())
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
			IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem> simulationWorldSystemsContainer,
			HorizonRunEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			simulationWorldSystemsContainer.Initialize(
				BuildSimulationWorldResolveSystems(
					entityManager,
					entityListManager,
					loggerResolver),
				BuildSimulationWorldInitializationSystems(
					eventEntityBuilder,
					entityListManager,
					simulationUpdateTimerManager,
					loggerResolver),
				BuildSimulationWorldDeinitializationSystems(
					entityManager,
					entityListManager,
					simulationUpdateTimerManager,
					loggerResolver));
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldResolveSystems(
			HorizonRunEntityManager entityManager,
			DefaultECSEntityListManager entityListManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Position
				new CopyPositionFromGameObjectResolveSystem<HorizonRunSceneEntity>(),

				// Rotation
				new CopyUniformRotationFromGameObjectResolveSystem<HorizonRunSceneEntity>(),
				new CopyQuaternionFromGameObjectResolveSystem<HorizonRunSceneEntity>(),

				// Sub spawners
				new LinkSubSpawnersResolveSystem<HorizonRunSceneEntity>(
					entityManager,
					entityListManager,
					loggerResolver?.GetLogger<LinkSubSpawnersResolveSystem<HorizonRunSceneEntity>>()));
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldInitializationSystems(
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Timers
				new LoopedTimerInitializationSystem(),

				// Hierarchy
				new HierarchyInitializationSystem(
					entityListManager),

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
					entityListManager));
		}

		public static IDefaultECSEntityInitializationSystem BuildSimulationWorldDeinitializationSystems(
			HorizonRunEntityManager entityManager,
			DefaultECSEntityListManager entityListManager,
			ITimerManager simulationUpdateTimerManager,
			ILoggerResolver loggerResolver = null)
		{
			return new DefaultECSSequentialEntityInitializationSystem(

				// Hierarchy
				new HierarchyDeinitializationSystem<GUIDComponent, Guid>(
					entityManager,
					entityListManager,
					(GUIDComponent) =>
					{
						return GUIDComponent.GUID;
					},
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
			HorizonRunEntityManager entityManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			DefaultECSEntityListManager entityListManager,
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
					entityManager),

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
			HorizonRunEntityManager entityManager)
		{
			return new SequentialSystem<float>(
				new CreateEntityOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager,
					authoringPreset),
				new PlaceEntityOnEntitySpawnedEventSystem(
					eventWorld,
					entityManager),
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
			ILoggerResolver loggerResolver = null)
		{
			return new SequentialSystem<float>(

				//Sensors
				new SensorWithTimerSystem(
					simulationWorld,
					simulationUpdateTimerManager,
					loggerResolver?.GetLogger<SensorWithTimerSystem>()));
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
			World simulationWorld)
		{
			return new SequentialSystem<float>(

					// Steering
					new LookTowardsLastLocomotion2DVectorSystem(
						simulationWorld),
					new LookTowardsLastLocomotion3DVectorSystem(
						simulationWorld),

					// Rotation
					new UniformRotationSystem(
						simulationWorld),
					new QuaternionRotationSystem(
						simulationWorld),

					// Locomotion
					new Locomotion2DSystem(
						simulationWorld),
					new Locomotion2DMemorySystem(
						simulationWorld),
					new Locomotion3DSystem(
						simulationWorld),
					new Locomotion3DMemorySystem(
						simulationWorld));
		}

		#endregion

		#region Late update systems

		public static ISystem<float> BuildLateUpdateSystems(
			World viewWorld,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			Camera mainRenderCamera,
			IUIManager uiManager)
		{
			return new SequentialSystem<float>(

				BuildViewInputSystems(
					viewWorld,
					mainRenderCamera,
					uiManager),

				BuildViewPresenterSystems(
					viewWorld,
					eventEntityBuilder),

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
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
		{
			return new SequentialSystem<float>(

				//Cursor presenters
				//TODO
				//new CursorSwitcherPresenterSystem(
				//	viewWorld,
				//	entityManager),

				//Controls presenters
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

				//Transform view presenters
				new TransformPositionPresenterSystem(
					viewWorld),
				new UniformRotationPresenterSystem(
					viewWorld),
				new QuaternionPresenterSystem(
					viewWorld),

				//Camera presenters
				new VirtualCameraPresenterSystem(
					viewWorld));
		}

		private static ISystem<float> BuildViewVisualsSystems(
			World viewWorld)
		{
			return new SequentialSystem<float>(

				//Transform views
				new TransformPositionViewSystem(
					viewWorld),
				new TransformUniformRotationViewSystem(
					viewWorld),
				new TransformQuaternionViewSystem(
					viewWorld));
		}

		#endregion
	}
}