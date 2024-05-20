using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleAnimatorPresenterInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly SampleEntityManager sampleEntityManager;

		private readonly ILogger logger;

		public SampleAnimatorPresenterInitializationSystem(
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

			if (!entity.Has<SampleAnimatorPresenterComponent>())
				return;

			ref var sampleAnimatorPresenterComponent = ref entity.Get<SampleAnimatorPresenterComponent>();

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

			sampleAnimatorPresenterComponent.TargetEntity = simulationEntity;
		}

		public void Dispose()
		{
		}
	}
}