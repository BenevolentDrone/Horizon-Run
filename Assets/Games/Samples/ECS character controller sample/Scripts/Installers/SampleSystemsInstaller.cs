using HereticalSolutions.Pools;

using HereticalSolutions.Time;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Entities;

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
		private SampleEntityManager entityManager;

		[Inject(Id = "Update time manager")]
		private ITimeManager updateTimeManager;

		[Inject(Id = "Fixed update time manager")]
		private ITimeManager fixedUpdateTimeManager;

		[Inject(Id = "Late update time manager")]
		private ITimeManager lateUpdateTimeManager;

		[Inject]
		private INonAllocDecoratedPool<GameObject> gameObjectPool;

		[SerializeField]
		private SampleECSUpdateBehaviour sampleECSUpdateBehaviour;

		public override void InstallBindings()
		{
			//var logger = loggerResolver.GetLogger<SampleEntityPrototypeImportInstaller>();

			#region Resolve and initialization systems

			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldsRepository = worldContainer.EntityWorldsRepository;

			var viewWorldController = entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

			var viewWorldSystemsContainer = viewWorldController as  IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem>;

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


			var simulationWorld = entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID);

			var viewWorld = entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID);


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
		}
	}
}