using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SamplePositionPresenterInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly EntityManager entityManager;

		private readonly ILogger logger;

		public SamplePositionPresenterInitializationSystem(
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

			if (!entity.Has<SamplePositionPresenterComponent>())
				return;

			ref var samplePositionPresenterComponent = ref entity.Get<SamplePositionPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			if (!entityManager.TryGetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID,
				out var simulationEntity))
			{
				logger?.LogError<SamplePositionPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<SamplePositionPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			samplePositionPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}