using System;
using HereticalSolutions.Pools;

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
		/*
		private const string LOCK_ENTITY_PROTOTYPE_ID = "Lock";

		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private EntityAuthoringSettings entityAuthoringSettings;

		[Inject(Id = "Main render camera")]
		private Camera mainRenderCamera;

		[Inject]
		private IUIManager uiManager;
		
		[Inject]
		private IVFXManager vfxManager; 

		[Inject]
		private DefaultECSEntityListManager entityListManager;

		[Inject]
		private DefaultECSEntityHierarchyManager entityHierarchyManager;

		[Inject]
		private UniversalTemplateEntityManager entityManager;
		
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
		private IManagedPool<GameObject> gameObjectPool;
		*/

        //[SerializeField]
        //private UniversalTemplateSimulationBehaviour simulationBehaviour;

		[SerializeField]
		private Transform hudCanvasTransform;
        
		public override void InstallBindings()
		{
			/*
			var logger = loggerResolver?.GetLogger<SystemsInstaller>();

			var authoringPreset = entityAuthoringSettings.AuthoringPreset;

			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;
			
			bool includeViewWorld =
				authoringPreset == EEntityAuthoringPresets.DEFAULT
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_HOST
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_CLIENT;


			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldRepository = worldContainer.EntityWorldRepository;
			
			
			var eventWorld = entityWorldRepository.GetWorld(WorldConstants.EVENT_WORLD_ID);

			var simulationWorld = (includeSimulationWorld)
				? entityWorldRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID)
				: default;

			var viewWorld = (includeViewWorld)
				? entityWorldRepository.GetWorld(WorldConstants.VIEW_WORLD_ID)
				: default;
			
			#region Synchronization providers

			var updateSynchronizationProviderRepository = updateTimeManager as ISynchronizationProviderRepository;

			updateSynchronizationProviderRepository.TryGetProvider(
				"Update",
				out var updateSynchronizationProvider);


			var fixedUpdateSynchronizationProviderRepository = fixedUpdateTimeManager as ISynchronizationProviderRepository;

			fixedUpdateSynchronizationProviderRepository.TryGetProvider(
				"Fixed update",
				out var fixedUpdateSynchronizationProvider);


			var lateUpdateSynchronizationProviderRepository = lateUpdateTimeManager as ISynchronizationProviderRepository;

			lateUpdateSynchronizationProviderRepository.TryGetProvider(
				"Late update",
				out var lateUpdateSynchronizationProvider);
			
			#endregion

			#region Resolve and initialization systems

			#region View world intitialization systems

			if (includeViewWorld)
			{
				var viewWorldController = entityWorldRepository.GeTEntityWorldController(WorldConstants.VIEW_WORLD_ID);

				var viewWorldSystemsContainer =
					viewWorldController as IEntityWorldControllerWithLifeCycleSystems<IEntityInitializationSystem>;

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
				var simulationWorldController = entityWorldRepository.GeTEntityWorldController(WorldConstants.SIMULATION_WORLD_ID);

				var simulationWorldSystemsContainer = simulationWorldController as 
					IEntityWorldControllerWithLifeCycleSystems<IEntityInitializationSystem>;

				SystemsFactory.InitializeSimulationWorldSystemsContainer(
					simulationWorld,
					simulationWorldSystemsContainer,
					entityManager,
					eventEntityBuilder,
					entityListManager,
					entityHierarchyManager,
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
				entityHierarchyManager,
				includeViewWorld,
				loggerResolver);

			#endregion

			#region Update systems

			ISystem<float> updateSystems = (includeSimulationWorld)
				? SystemsFactory.BuildUpdateSystems(
					simulationWorld,
					viewWorld,
					simulationUpdateTimerManager,
					entityListManager,
					entityHierarchyManager,
					includeViewWorld,
					loggerResolver)
				: new PlaceholderSystem();

			#endregion

			#region Fixed update systems

			ISystem<float> fixedUpdateSystems = (includeSimulationWorld)
				? SystemsFactory.BuildFixedUpdateSystems(
					simulationWorld,
					entityHierarchyManager,
					loggerResolver)
				: new PlaceholderSystem();

			#endregion

			#region Late systems

			ISystem<float> lateUpdateSystems = (includeViewWorld)
				? SystemsFactory.BuildLateUpdateSystems(
					viewWorld,
					entityManager,
					eventEntityBuilder,
					entityHierarchyManager,
					mainRenderCamera,
					uiManager,
					loggerResolver)
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
				
				loggerResolver?.GetLogger<UniversalTemplateSimulationBehaviour>());

			simulationBehaviour.StartSimulation();
			*/
		}
	}
}