using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class Position2DPresenterInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly UniversalTemplateEntityManager entityManager;

		private readonly ILogger logger;

		public Position2DPresenterInitializationSystem(
			UniversalTemplateEntityManager entityManager,
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

			if (!entity.Has<Position2DPresenterComponent>())
				return;

			ref var positionPresenterComponent = ref entity.Get<Position2DPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			var simulationEntity = entityManager.GetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID);

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<Position2DPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			positionPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
	*/
}