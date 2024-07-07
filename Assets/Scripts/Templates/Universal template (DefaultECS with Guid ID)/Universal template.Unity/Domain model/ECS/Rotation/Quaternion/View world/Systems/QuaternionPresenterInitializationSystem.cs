using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class QuaternionPresenterInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;

        private readonly ILogger logger;

        public QuaternionPresenterInitializationSystem(
            UniversalTemplateEntityManager entityManager,
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

            if (!entity.Has<QuaternionPresenterComponent>())
                return;

            ref var quaternionPresenterComponent = ref entity.Get<QuaternionPresenterComponent>();

            var guid = entity.Get<GUIDComponent>().GUID;

            var simulationEntity = entityManager.GetEntity(
                guid,
                WorldConstants.SIMULATION_WORLD_ID);

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