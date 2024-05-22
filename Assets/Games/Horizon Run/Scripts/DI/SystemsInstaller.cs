using System;
//using System.Collections.Generic;

using HereticalSolutions.Pools;

//using HereticalSolutions.Repositories;

//using HereticalSolutions.SpacePartitioning;

using HereticalSolutions.Time;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
	public class SystemsInstaller : MonoInstaller
	{
		private const string LOCK_ENTITY_PROTOTYPE_ID = "Lock";

		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private EEntityAuthoringPresets authoringPreset;

		[Inject(Id = "Main render camera")]
		private Camera mainRenderCamera;

		[Inject]
		private IUIManager uiManager;
		
		[Inject]
		private IVFXManager vfxManager; 

		[Inject]
		private DefaultECSEntityListManager entityListManager;

		[Inject]
		private HorizonRunEntityManager entityManager;
		
		[Inject]
		private IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		
		[Inject(Id = "Update time manager")]
		private ITimeManager updateTimeManager;

		[Inject(Id = "Fixed update time manager")]
		private ITimeManager fixedUpdateTimeManager;

		[Inject(Id = "Late update time manager")]
		private ITimeManager lateUpdateTimeManager;
		
		
		[Inject(Id = "Simulation update timer manager")]
		private ITimerManager simulationUpdateTimerManager;

		[Inject(Id = "Simulation fixed update timer manager")]
		private ITimerManager simulationFixedUpdateTimerManager;

		[Inject(Id = "View late update timer manager")]
		private ITimerManager viewLateUpdateTimerManager;
		

		[Inject]
		private INonAllocDecoratedPool<GameObject> gameObjectPool;


		/*
		[SerializeField]
		private SystemSettings systemSettings;
		*/

        [SerializeField]
        private HorizonRunSimulationBehaviour simulationBehaviour;

		[SerializeField]
		private Transform hudCanvasTransform;
        
		/*
        [SerializeField]
        private HorizonRunTimeSynchronizationBehaviour timeSynchronizationBehaviour;
		*/

		public override void InstallBindings()
		{
			var logger = loggerResolver?.GetLogger<SystemsInstaller>();

			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;
			
			bool includeViewWorld =
				authoringPreset == EEntityAuthoringPresets.DEFAULT
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_HOST
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_CLIENT;


			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldsRepository = worldContainer.EntityWorldsRepository;
			
			
			var eventWorld = entityWorldsRepository.GetWorld(WorldConstants.EVENT_WORLD_ID);

			var simulationWorld = (includeSimulationWorld)
				? entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID)
				: default;

			var viewWorld = (includeViewWorld)
				? entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID)
				: default;
			
			#region Synchronization providers

			var updateSynchronizationProvidersRepository = updateTimeManager as ISynchronizationProvidersRepository;

			updateSynchronizationProvidersRepository.TryGetProvider(
				"Update",
				out var updateSynchronizationProvider);


			var fixedUpdateSynchronizationProvidersRepository = fixedUpdateTimeManager as ISynchronizationProvidersRepository;

			fixedUpdateSynchronizationProvidersRepository.TryGetProvider(
				"Fixed update",
				out var fixedUpdateSynchronizationProvider);


			var lateUpdateSynchronizationProvidersRepository = lateUpdateTimeManager as ISynchronizationProvidersRepository;

			lateUpdateSynchronizationProvidersRepository.TryGetProvider(
				"Late update",
				out var lateUpdateSynchronizationProvider);
			
			#endregion

			#region Resolve and initialization systems

			#region View world intitialization systems

			if (includeViewWorld)
			{
				var viewWorldController = entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

				var viewWorldSystemsContainer =
					viewWorldController as IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem>;

				SystemsFactory.InitializeViewWorldSystemsContainer(
					viewWorldSystemsContainer,
					entityManager,
					gameObjectPool,
					hudCanvasTransform,
					loggerResolver);
			}

			#endregion

			#region Simulation world initialization systems

			if (includeSimulationWorld)
			{
				var simulationWorldController = entityWorldsRepository.GetWorldController(WorldConstants.SIMULATION_WORLD_ID);

				var simulationWorldSystemsContainer = simulationWorldController as 
					IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem>;

				SystemsFactory.InitializeSimulationWorldSystemsContainer(
					simulationWorldSystemsContainer,
					entityManager,
					eventEntityBuilder,
					entityListManager,
					simulationUpdateTimerManager,
					loggerResolver);
			}					

			#endregion

			#endregion
			
			#region Event systems

			ISystem<float> eventSystems = SystemsFactory.BuildEventWorldSystems(
				eventWorld,
				viewWorld,
				authoringPreset,
				entityManager,
				eventEntityBuilder,
				entityListManager,
				includeViewWorld,
				loggerResolver);

			#endregion

			#region Update systems

			ISystem<float> updateSystems = (includeSimulationWorld)
				? SystemsFactory.BuildUpdateSystems(
					simulationWorld,
					viewWorld,
					simulationUpdateTimerManager,
					includeViewWorld,
					loggerResolver)
				: new PlaceholderSystem();

			#endregion

			#region Fixed update systems

			ISystem<float> fixedUpdateSystems = (includeSimulationWorld)
				? SystemsFactory.BuildFixedUpdateSystems(
					simulationWorld)
				: new PlaceholderSystem();

			#endregion

			#region Late systems

			ISystem<float> lateUpdateSystems = (includeViewWorld)
				? SystemsFactory.BuildLateUpdateSystems(
					viewWorld,
					eventEntityBuilder,
					mainRenderCamera,
					uiManager)
				: new PlaceholderSystem();

			#endregion

			simulationBehaviour.Initialize(
				updateSynchronizationProvider,
				fixedUpdateSynchronizationProvider,
				lateUpdateSynchronizationProvider,

				eventSystems,
				updateSystems,
				fixedUpdateSystems,
				lateUpdateSystems,
				
				loggerResolver?.GetLogger<HorizonRunSimulationBehaviour>());

			simulationBehaviour.StartSimulation();
		}
	}
}