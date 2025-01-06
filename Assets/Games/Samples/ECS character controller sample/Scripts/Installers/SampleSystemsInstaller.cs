using HereticalSolutions.Pools;

using HereticalSolutions.Time;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleSystemsInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private EntityManager entityManager;

		[Inject(Id = "Update time manager")]
		private ITimeManager updateTimeManager;

		[Inject(Id = "Fixed update time manager")]
		private ITimeManager fixedUpdateTimeManager;

		[Inject(Id = "Late update time manager")]
		private ITimeManager lateUpdateTimeManager;

		[Inject]
		private IManagedPool<GameObject> gameObjectPool;

		[SerializeField]
		private SampleECSUpdateBehaviour sampleECSUpdateBehaviour;

		public override void InstallBindings()
		{
			/*
			//var logger = loggerResolver.GetLogger<SampleEntityPrototypeImportInstaller>();

			#region Resolve and initialization systems

			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldRepository = worldContainer.EntityWorldRepository;

			var viewWorldController = entityWorldRepository.GeTEntityWorldController(WorldConstants.VIEW_WORLD_ID);

			var viewWorldSystemsContainer = viewWorldController as  IEntityWorldControllerWithLifeCycleSystems<IEntityInitializationSystem>;

			viewWorldSystemsContainer.Initialize(
				new DefaultECSSequentialEntityInitializationSystem(
					new ResolvePooledGameObjectViewSystem<SampleSceneEntity>(
						gameObjectPool,
						loggerResolver?.GetLogger<ResolvePooledGameObjectViewSystem<SampleSceneEntity>>())),
				new DefaultECSSequentialEntityInitializationSystem(
					new SpawnPooledGameObjectViewSystem(
						gameObjectPool,
						loggerResolver?.GetLogger<SpawnPooledGameObjectViewSystem>()),

					new SamplePositionPresenterInitializationSystem(
						entityManager,
						loggerResolver?.GetLogger<SamplePositionPresenterInitializationSystem>()),
					new SampleRotationPresenterInitializationSystem(
						entityManager,
						loggerResolver?.GetLogger<SampleRotationPresenterInitializationSystem>()),
					new SampleAnimatorPresenterInitializationSystem(
						entityManager,
						loggerResolver?.GetLogger<SampleAnimatorPresenterInitializationSystem>())),
				null);

			#endregion

			#region Update systems

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


			var simulationWorld = entityWorldRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID);

			var viewWorld = entityWorldRepository.GetWorld(WorldConstants.VIEW_WORLD_ID);


			ISystem<float> updateSystems = new SequentialSystem<float>(
				new SampleJoystickPresenterInitializationSystem(
					viewWorld,
					simulationWorld),
				new SampleVirtualCameraPresenterInitializationSystem(
					viewWorld,
					simulationWorld,
					entityManager));

			ISystem<float> fixedUpdateSystems = new SequentialSystem<float>(
				new SampleLookTowardsLastLocomotionVectorSystem(
					simulationWorld),

				new SampleRotationSystem(
					simulationWorld),

				new SampleLocomotionSystem(
					simulationWorld),
				new SampleLocomotionMemorySystem(
					simulationWorld));			

			ISystem<float> lateUpdateSystems = new SequentialSystem<float>(
				new SampleJoystickPresenterSystem(
					viewWorld),

				new SamplePositionPresenterSystem(
					viewWorld),
				new SampleRotationPresenterSystem(
					viewWorld),
				new SampleAnimatorPresenterSystem(
					viewWorld),

				new SampleVirtualCameraPresenterSystem(
					viewWorld),

				new SampleTransformPositionViewSystem(
					viewWorld),
				new SampleTransformRotationViewSystem(
					viewWorld),
				new SampleAnimatorViewSystem(
					viewWorld));

			sampleECSUpdateBehaviour.Initialize(
				updateSynchronizationProvider,
				fixedUpdateSynchronizationProvider,
				lateUpdateSynchronizationProvider,

				updateSystems,
				fixedUpdateSystems,
				lateUpdateSystems);

			#endregion

			*/
		}
	}
}