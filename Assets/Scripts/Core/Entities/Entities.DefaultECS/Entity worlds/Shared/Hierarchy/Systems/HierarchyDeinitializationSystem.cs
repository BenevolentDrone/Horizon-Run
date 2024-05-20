using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>
        : IDefaultECSEntityInitializationSystem
    {
        private readonly DefaultECSEntityManager<TEntityID> entityManager;

        private readonly DefaultECSEntityListManager entityListManager;

        private readonly Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate;
        
        private readonly ILogger logger;

        public HierarchyDeinitializationSystem(
            DefaultECSEntityManager<TEntityID> entityManager,
            DefaultECSEntityListManager entityListManager,
            Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
            ILogger logger = null)
        {
            this.entityManager = entityManager;
            
            this.entityListManager = entityListManager;

            this.getEntityIDFromIDComponentDelegate = getEntityIDFromIDComponentDelegate;

            this.logger = logger;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<HierarchyComponent>())
                return;

            var hierarchyComponent = entity.Get<HierarchyComponent>();
            
            var childrenList = entityListManager.GetList(
                hierarchyComponent.ChildrenListID);

            if (childrenList == null)
            {
                //throw new Exception(
                //    logger.TryFormat<HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>>(
                //        $"ENTITY LIST {hierarchyComponent.ChildrenListID} NOT FOUND"));
                
                return;
            }
            
            //foreach (var child in childrenList)
            for (int i = childrenList.Count - 1; i >= 0; i--)
            {
                var child = childrenList[i];
                
                if (child.IsAlive)
                {
                    logger?.Log<HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>>(
                        $"DESPAWNING CHILD ENTITY {child} OF ENTITY {entity}");
                    
                    if (child.Has<TEntityIDComponent>())
                    {
                        var id = getEntityIDFromIDComponentDelegate.Invoke(
                            child.Get<TEntityIDComponent>());
                        
                        entityManager.DespawnEntity(
                            id);
                    }
                    else
                    {
                        entityManager.DespawnWorldLocalEntity(child);
                    }
                }
            }

            if (hierarchyComponent.Parent.IsAlive)
            {
                logger?.Log<HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>>(
                    $"DETACHING ENTITY {entity} FROM PARENT ENTITY {hierarchyComponent.Parent}");
                
                HierarchyHelpers.RemoveChild(
                    hierarchyComponent.Parent,
                    entity,
                    entityListManager,
                    logger);
            }

            entityListManager.RemoveList(
                hierarchyComponent.ChildrenListID);
        }

        public void Dispose()
        {
        }
    }
}