using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPositionRotationPresenter3DInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;

        private readonly ILogger logger;

        public ServerPositionRotationPresenter3DInitializationSystem(
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

            if (!entity.Has<ServerPositionRotation3DPresenterComponent>())
                return;

            ref var serverPositionRotation3DPresenterComponent = ref entity.Get<ServerPositionRotation3DPresenterComponent>();

            var guid = entity.Get<GUIDComponent>().GUID;

            var simulationEntity = entityManager.GetEntity(
                guid,
                WorldConstants.SIMULATION_WORLD_ID);

            if (!simulationEntity.IsAlive)
            {
                logger?.LogError<ServerPositionRotationPresenter3DInitializationSystem>(
                    $"ENTITY {guid} HAS NO SIMULATION ENTITY");

                return;
            }
            
            if (!simulationEntity.Has<Position3DComponent>())
                return;
            
            if (!simulationEntity.Has<QuaternionComponent>())
                return;
            
            if (!simulationEntity.Has<ServerPosition3DComponent>())
                return;
            
            if (!simulationEntity.Has<ServerQuaternionComponent>())
                return;

            serverPositionRotation3DPresenterComponent.TargetEntity = simulationEntity;
        }

        public void Dispose()
        {
        }
    }
}