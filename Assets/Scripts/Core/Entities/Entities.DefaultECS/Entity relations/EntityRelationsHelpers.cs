using System;

using HereticalSolutions.Relations;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public static class EntityRelationsHelpers
    {
        public static void EstablishRelations<TEntityID>(
            Entity from,
            Entity to,
            string key,
            DefaultECSEntityRelationsManager entityRelationsManager,
            bool force = true,
            ILogger logger = null)
        {
            if (!from.Has<RelationsComponent>())
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} DOES NOT HAVE A RELATIONS COMPONENT"));

            if (!to.Has<RelationsComponent>())
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {to} DOES NOT HAVE A RELATIONS COMPONENT"));

            var fromEntityRelationsComponent = from.Get<RelationsComponent>();

            if (!entityRelationsManager.TryGet(
                    fromEntityRelationsComponent.RelationsHandle,
                    out var fromNode))
            {
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} RELATIONS NODE INVALID: {fromEntityRelationsComponent.RelationsHandle}"));
            }

            ref var toEntityRelationsComponent = ref to.Get<RelationsComponent>();

            if (!entityRelationsManager.TryGet(
                    toEntityRelationsComponent.RelationsHandle,
                    out var toNode))
            {
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {to} RELATIONS NODE INVALID: {toEntityRelationsComponent.RelationsHandle}"));
            }

            var fromNodeAsMutable = fromNode as IDirectedNamedGraphNode<Entity>;

            if (fromNodeAsMutable == null)
                return;

            if (force)
            {
                fromNodeAsMutable.AddOrReplaceRelation(
                    key,
                    toNode);
            }
            else
            {
                fromNodeAsMutable.TryAddRelation(
                    key,
                    toNode);
            }
        }
        
        public static void AbandonRelations<TEntityID>(
            Entity from,
            string key,
            DefaultECSEntityRelationsManager entityRelationsManager,
            ILogger logger = null)
        {
            if (!from.Has<RelationsComponent>())
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} DOES NOT HAVE A RELATIONS COMPONENT"));

            var fromEntityRelationsComponent = from.Get<RelationsComponent>();

            if (!entityRelationsManager.TryGet(
                    fromEntityRelationsComponent.RelationsHandle,
                    out var fromNode))
            {
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} RELATIONS NODE INVALID: {fromEntityRelationsComponent.RelationsHandle}"));
            }

            var fromNodeAsMutable = fromNode as IDirectedNamedGraphNode<Entity>;

            if (fromNodeAsMutable == null)
                return;

            fromNodeAsMutable.TryRemoveRelation(
                key);
        }

        public static bool GetRelative(
            Entity from,
            out Entity to,
            string relationPath,
            DefaultECSEntityRelationsManager entityRelationsManager,
            ILogger logger = null)
        {
            string[] relationPathParts = relationPath.SplitAddressBySeparator();
            
            return GetRelative(
                from,
                out to,
                relationPathParts,
                entityRelationsManager,
                logger);
        }

        public static bool GetRelative(
            Entity from,
            out Entity to,
            string[] relationChain,
            DefaultECSEntityRelationsManager entityRelationsManager,
            ILogger logger = null)
        {
            if (relationChain == null
                || relationChain.Length == 0)
            {
                to = default;
                
                return false;
            }
            
            if (!from.Has<RelationsComponent>())
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} DOES NOT HAVE A RELATIONS COMPONENT"));

            var fromEntityRelationsComponent = from.Get<RelationsComponent>();

            if (!entityRelationsManager.TryGet(
                fromEntityRelationsComponent.RelationsHandle,
                out var fromNode))
            {
                throw new Exception(
                    logger.TryFormatException(
                        $"ENTITY {from} RELATIONS NODE INVALID: {fromEntityRelationsComponent.RelationsHandle}"));
            }

            var currentNode = fromNode;

            foreach (var relationKey in relationChain)
            {
                if (!currentNode.HasRelation(relationKey))
                {
                    logger?.LogError(
                        $"RELATION {relationKey} NOT FOUND");

                    to = default;
                    
                    return false;
                }
                
                currentNode = currentNode.GetRelation(
                    relationKey);
            }

            to = currentNode.Contents;

            return true;
        }
    }
}