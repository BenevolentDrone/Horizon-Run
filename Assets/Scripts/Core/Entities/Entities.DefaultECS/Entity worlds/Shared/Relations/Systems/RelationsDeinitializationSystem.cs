using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class RelationsDeinitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly DefaultECSEntityRelationsManager entityRelationsManager;
        
        private readonly ILogger logger;

        public RelationsDeinitializationSystem(
            DefaultECSEntityRelationsManager entityRelationsManager,
            ILogger logger = null)
        {
            this.entityRelationsManager = entityRelationsManager;

            this.logger = logger;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<RelationsComponent>())
                return;

            var relationsComponent = entity.Get<RelationsComponent>();

            entityRelationsManager.TryRemove(
                relationsComponent.RelationsHandle);
        }

        public void Dispose()
        {
        }
    }
}