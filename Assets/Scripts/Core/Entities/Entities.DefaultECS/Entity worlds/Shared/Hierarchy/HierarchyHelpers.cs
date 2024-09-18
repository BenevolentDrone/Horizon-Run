using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;

using DefaultEcs;
using HereticalSolutions.Hierarchy;

namespace HereticalSolutions.Entities
{
    public static class HierarchyHelpers
    {
        public static bool TryGetParent(
                    Entity child,
                    DefaultECSEntityHierarchyManager entityHierarchyManager,
                    out Entity parent,
                    ILogger logger = null)
        {
            parent = default;

            if (!child.Has<HierarchyComponent>())
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} DOES NOT HAVE A HIERARCHY COMPONENT");

                return false;
            }

            var childHierarchyComponent = child.Get<HierarchyComponent>();

            if (!entityHierarchyManager.TryGet(
                childHierarchyComponent.HierarchyHandle,
                out var childNode))
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} HIERARCHY NODE INVALID: {childHierarchyComponent.HierarchyHandle}");

                return false;
            }

            if (childNode.Parent == null)
                return false;

            var parentNode = childNode.Parent;

            if (parentNode.Contents == null)
                return false;

            parent = parentNode.Contents;

            return true;
        }

        public static void AddChild(
            Entity parent,
            Entity child,
            DefaultECSEntityHierarchyManager entityHierarchyManager,
            ILogger logger = null)
        {
            if (!parent.Has<HierarchyComponent>())
            {
                logger?.LogError(
                    $"PARENT ENTITY {parent} DOES NOT HAVE A HIERARCHY COMPONENT");
                
                return;
            }

            if (!child.Has<HierarchyComponent>())
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} DOES NOT HAVE A HIERARCHY COMPONENT");
                
                return;
            }

            var parentHierarchyComponent = parent.Get<HierarchyComponent>();

            if (!entityHierarchyManager.TryGet(
                parentHierarchyComponent.HierarchyHandle,
                out var parentNode))
            {
                logger?.LogError(
                    $"PARENT ENTITY {parent} HIERARCHY NODE INVALID: {parentHierarchyComponent.HierarchyHandle}");
                
                return;
            }

            ref var childHierarchyComponent = ref child.Get<HierarchyComponent>();
            
            if (!entityHierarchyManager.TryGet(
                childHierarchyComponent.HierarchyHandle,
                out var childNode))
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} HIERARCHY NODE INVALID: {childHierarchyComponent.HierarchyHandle}");
                
                return;
            }
            
            (parentNode as IHierarchyNode<Entity>)?.AddChild(childNode);
        }

        public static void RemoveChild(
            Entity parent,
            Entity child,
            DefaultECSEntityHierarchyManager entityHierarchyManager,
            ILogger logger = null)
        {
            if (!parent.Has<HierarchyComponent>())
            {
                logger?.LogError(
                    $"PARENT ENTITY {parent} DOES NOT HAVE A HIERARCHY COMPONENT");
                
                return;
            }

            if (!child.Has<HierarchyComponent>())
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} DOES NOT HAVE A HIERARCHY COMPONENT");
                
                return;
            }

            var parentHierarchyComponent = parent.Get<HierarchyComponent>();

            if (!entityHierarchyManager.TryGet(
                    parentHierarchyComponent.HierarchyHandle,
                    out var parentNode))
            {
                logger?.LogError(
                    $"PARENT ENTITY {parent} HIERARCHY NODE INVALID: {parentHierarchyComponent.HierarchyHandle}");
                
                return;
            }

            ref var childHierarchyComponent = ref child.Get<HierarchyComponent>();
            
            if (!entityHierarchyManager.TryGet(
                    childHierarchyComponent.HierarchyHandle,
                    out var childNode))
            {
                logger?.LogError(
                    $"CHILD ENTITY {child} HIERARCHY NODE INVALID: {childHierarchyComponent.HierarchyHandle}");
                
                return;
            }
            
            (parentNode as IHierarchyNode<Entity>)?.RemoveChild(childNode);
        }
    }
}