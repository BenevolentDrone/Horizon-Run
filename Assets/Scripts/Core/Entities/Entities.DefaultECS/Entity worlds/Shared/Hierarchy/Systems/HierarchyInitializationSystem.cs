using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class HierarchyInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly DefaultECSEntityListManager entityListManager;

        public HierarchyInitializationSystem(
            DefaultECSEntityListManager entityListManager)
        {
            this.entityListManager = entityListManager;
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

            entityListManager.CreateList(
                out var entityListID,
                out var entityList);

            hierarchyComponent.ChildrenListID = entityListID;
        }

        public void Dispose()
        {
        }
    }
}