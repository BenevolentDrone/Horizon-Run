using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class QuaternionPresenterInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly EntityManager entityManager;

        private readonly ILogger logger;

        public QuaternionPresenterInitializationSystem(
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

            if (!entity.Has<QuaternionPresenterComponent>())
                return;

            ref var quaternionPresenterComponent = ref entity.Get<QuaternionPresenterComponent>();

            var guid = entity.Get<GUIDComponent>().GUID;

            if (!entityManager.TryGetEntity(
                guid,
                WorldConstants.SIMULATION_WORLD_ID,
                out var simulationEntity))
            {
                logger?.LogError<QuaternionPresenterInitializationSystem>(
                    $"ENTITY {guid} HAS NO SIMULATION ENTITY");

                return;
            }

            if (!simulationEntity.IsAlive)
            {
                logger?.LogError<QuaternionPresenterInitializationSystem>(
                    $"ENTITY {guid} HAS NO SIMULATION ENTITY");

                return;
            }

            quaternionPresenterComponent.TargetEntity = simulationEntity;
        }

        public void Dispose()
        {
        }
    }
}