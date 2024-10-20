using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleAnimatorPresenterInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly EntityManager entityManager;

		private readonly ILogger logger;

		public SampleAnimatorPresenterInitializationSystem(
			EntityManager entityManager,
			ILogger logger = null)
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

			if (!entity.Has<SampleAnimatorPresenterComponent>())
				return;

			ref var sampleAnimatorPresenterComponent = ref entity.Get<SampleAnimatorPresenterComponent>();

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

			sampleAnimatorPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}