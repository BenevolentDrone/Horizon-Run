using HereticalSolutions.Hierarchy;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class HierarchyInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

        public HierarchyInitializationSystem(
            DefaultECSEntityHierarchyManager entityHierarchyManager)
        {
            this.entityHierarchyManager = entityHierarchyManager;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<HierarchyComponent>())
                return;

            ref var hierarchyComponent = ref entity.Get<HierarchyComponent>();

            entityHierarchyManager.TryAllocate(
                out var entityHierarchyHandle,
                out var node);

            hierarchyComponent.HierarchyHandle = entityHierarchyHandle;

            ((IHierarchyNode<Entity>)node).Contents = entity;
        }

        public void Dispose()
        {
        }
    }
}