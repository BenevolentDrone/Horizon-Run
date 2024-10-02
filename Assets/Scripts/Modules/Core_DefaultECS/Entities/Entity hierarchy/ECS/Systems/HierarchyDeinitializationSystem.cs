using System;
using System.Collections.Generic;

using HereticalSolutions.Pools;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>
        : IEntityInitializationSystem
    {
        private readonly EntityManager entityManager;

        private readonly EntityHierarchyManager entityHierarchyManager;

        private readonly IPool<List<IReadOnlyHierarchyNode<Entity>>> bufferPool;
        
        private readonly ILogger logger;

        public HierarchyDeinitializationSystem(
            EntityManager entityManager,
            EntityHierarchyManager entityHierarchyManager,
            IPool<List<IReadOnlyHierarchyNode<Entity>>> bufferPool,
            ILogger logger = null)
        {
            this.entityManager = entityManager;
            
            this.entityHierarchyManager = entityHierarchyManager;

            this.bufferPool = bufferPool;

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

            if (!entityHierarchyManager.TryGet(
                hierarchyComponent.HierarchyHandle,
                out var node))
            {
                return;
            }

            var childNodes = bufferPool.Pop();
            
            childNodes.AddRange(node.Children);

            foreach (var childNode in childNodes)
            {
                var childEntity = childNode.Contents;
                
                if (childEntity.IsAlive)
                {
                    logger?.Log<HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>>(
                        $"DESPAWNING CHILD ENTITY {childEntity} OF ENTITY {entity}");
                    
                    if (childEntity.Has<TEntityIDComponent>())
                    {
                        var id =  childEntity.Get<GUIDComponent>().GUID;
                        
                        entityManager.DespawnEntity(
                            id);
                    }
                    else
                    {
                        entityManager.DespawnWorldLocalEntity(childEntity);
                    }
                }
            }
            
            bufferPool.Push(childNodes);

            if (node.Parent != null
                && node.Parent.Contents.IsAlive)
            {
                logger?.Log<HierarchyDeinitializationSystem<TEntityIDComponent, TEntityID>>(
                    $"DETACHING ENTITY {entity} FROM PARENT ENTITY {node.Parent.Contents}");
            }

            entityHierarchyManager.TryRemove(
                hierarchyComponent.HierarchyHandle);
        }

        public void Dispose()
        {
        }
    }
}