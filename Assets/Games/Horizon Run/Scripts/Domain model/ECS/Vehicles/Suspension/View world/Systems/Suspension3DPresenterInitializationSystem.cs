using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class Suspension3DPresenterInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly HorizonRunEntityManager entityManager;

		private readonly ILogger logger;

		public Suspension3DPresenterInitializationSystem(
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

			if (!entity.Has<Suspension3DPresenterComponent>())
				return;

			ref var suspensionPresenterComponent = ref entity.Get<Suspension3DPresenterComponent>();

			var guid = entity.Get<GUIDComponent>().GUID;

			var simulationEntity = entityManager.GetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID);

			if (!simulationEntity.IsAlive)
			{
				logger?.LogError<Suspension3DPresenterInitializationSystem>(
					$"ENTITY {guid} HAS NO SIMULATION ENTITY");

				return;
			}

			suspensionPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}