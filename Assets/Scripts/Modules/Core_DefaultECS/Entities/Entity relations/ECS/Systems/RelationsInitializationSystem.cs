/*
using HereticalSolutions.Relations;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class RelationsInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly EntityRelationsManager entityRelationsManager;

        public RelationsInitializationSystem(
            EntityRelationsManager entityRelationsManager)
        {
            this.entityRelationsManager = entityRelationsManager;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<RelationsComponent>())
                return;

            ref var relationsComponent = ref entity.Get<RelationsComponent>();

            entityRelationsManager.TryAllocate(
                out var entityRelationsHandle,
                out var node);

            relationsComponent.RelationsHandle = entityRelationsHandle;

            ((IDirectedNamedGraphNode<Entity>)node).Contents = entity;
        }

        public void Dispose()
        {
        }
    }
}
*/