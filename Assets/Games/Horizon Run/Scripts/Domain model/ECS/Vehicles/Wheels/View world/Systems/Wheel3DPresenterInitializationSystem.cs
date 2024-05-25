using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class Wheel3DPresenterInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly HorizonRunEntityManager entityManager;

		private readonly ILogger logger;

		public Wheel3DPresenterInitializationSystem(
			HorizonRunEntityManager entityManager,
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

			if (!entity.Has<Wheel3DPresenterComponent>())
				return;

			ref var wheelPresenterComponent = ref entity.Get<Wheel3DPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			var simulationEntity = entityManager.GetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID);

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<Wheel3DPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			wheelPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}