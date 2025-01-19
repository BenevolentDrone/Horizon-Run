using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class Position3DPresenterInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly EntityManager entityManager;

		private readonly ILogger logger;

		public Position3DPresenterInitializationSystem(
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

			if (!entity.Has<Position3DPresenterComponent>())
				return;

			ref var positionPresenterComponent = ref entity.Get<Position3DPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			if (!entityManager.TryGetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID,
				out var simulationEntity))
			{
				logger?.LogError<Position3DPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<Position3DPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			positionPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}