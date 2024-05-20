using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleRotationPresenterInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly SampleEntityManager sampleEntityManager;

		private readonly ILogger logger;

		public SampleRotationPresenterInitializationSystem(
			SampleEntityManager sampleEntityManager,
			ILogger logger = null)
		{
			this.sampleEntityManager = sampleEntityManager;

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

			var simulationEntity = sampleEntityManager.GetEntity(
				guid,
				WorldConstants.SIMULATION_WORLD_ID);

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