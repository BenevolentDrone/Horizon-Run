using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class RelationsDeinitializationSystem
        : IEntityInitializationSystem
    {
        private readonly EntityRelationsManager entityRelationsManager;
        
        private readonly ILogger logger;

        public RelationsDeinitializationSystem(
            EntityRelationsManager entityRelationsManager,
            ILogger logger)
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