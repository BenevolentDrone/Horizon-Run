using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public static class HierarchyHelpers
    {
        public static void AddChild(
            Entity parent,
            Entity child,
            DefaultECSEntityListManager entityListManager,
            ILogger logger = null)
        {
            if (!parent.Has<HierarchyComponent>())
                throw new Exception(
                    logger.TryFormat(
                        $"ENTITY {parent} DOES NOT HAVE A HIERARCHY COMPONENT"));
            
            if (!child.Has<HierarchyComponent>())
                throw new Exception(
                    logger.TryFormat(
                        $"ENTITY {child} DOES NOT HAVE A HIERARCHY COMPONENT"));
            
            var parentHierarchyComponent = parent.Get<HierarchyComponent>();
            
            entityListManager.GetOrCreateList(
                ref parentHierarchyComponent.ChildrenListID,
                out var parentsChildrenList);
            
            ref var childHierarchyComponent = ref child.Get<HierarchyComponent>();
            
            childHierarchyComponent.Parent = parent;
            
            if (!parentsChildrenList.Contains(child))
                parentsChildrenList.Add(child);
        }

        public static void RemoveChild(
            Entity parent,
            Entity child,
            DefaultECSEntityListManager entityListManager,
            ILogger logger = null)
        {
            if (!parent.Has<HierarchyComponent>())
                throw new Exception(
                    logger.TryFormat(
                        $"ENTITY {parent} DOES NOT HAVE A HIERARCHY COMPONENT"));
            
            var parentHierarchyComponent = parent.Get<HierarchyComponent>();

            if (!entityListManager.TryGetList(
                parentHierarchyComponent.ChildrenListID,
                out var parentsChildrenList))
            {
                return;
            }

            parentsChildrenList.Remove(child);

            if (!child.Has<HierarchyComponent>())
                return;
            
            ref var childHierarchyComponent = ref child.Get<HierarchyComponent>();
            
            childHierarchyComponent.Parent = default;
        }
    }
}