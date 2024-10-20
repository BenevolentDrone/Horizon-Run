using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    /*
    public class ServerPositionRotationPresenter2DInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;

        private readonly ILogger logger;

        public ServerPositionRotationPresenter2DInitializationSystem(
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

            if (!entity.Has<ServerPositionRotation2DPresenterComponent>())
                return;

            ref var serverPositionRotation2DPresenterComponent = ref entity.Get<ServerPositionRotation2DPresenterComponent>();

            var guid = entity.Get<GUIDComponent>().GUID;

            var simulationEntity = entityManager.GetEntity(
                guid,
                WorldConstants.SIMULATION_WORLD_ID);

            if (!simulationEntity.IsAlive)
            {
                logger?.LogError<ServerPositionRotationPresenter2DInitializationSystem>(
                    $"ENTITY {guid} HAS NO SIMULATION ENTITY");

                return;
            }
            
            if (!simulationEntity.Has<Position2DComponent>())
                return;
            
            if (!simulationEntity.Has<UniformRotationComponent>())
                return;
            
            if (!simulationEntity.Has<ServerPosition2DComponent>())
                return;
            
            if (!simulationEntity.Has<ServerUniformRotationComponent>())
                return;

            serverPositionRotation2DPresenterComponent.TargetEntity = simulationEntity;
        }

        public void Dispose()
        {
        }
    }
    */
}