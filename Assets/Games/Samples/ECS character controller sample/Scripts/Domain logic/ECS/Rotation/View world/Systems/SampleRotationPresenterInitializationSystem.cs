using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleRotationPresenterInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly EntityManager entityManager;

		private readonly ILogger logger;

		public SampleRotationPresenterInitializationSystem(
			EntityManager entityManager,
			ILogger logger)
		{
			this.entityManager = entityManager;

			this.logger = logger;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<SampleRotationPresenterComponent>())
				return;

			ref var sampleRotationPresenterComponent = ref entity.Get<SampleRotationPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			if (!entityManager.TryGetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID,
				out var simulationEntity))
			{
				logger?.LogError<SampleRotationPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<SampleRotationPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			sampleRotationPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}