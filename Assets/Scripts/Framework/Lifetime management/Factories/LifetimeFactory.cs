using System;
using System.Collections.Generic;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement.Factories
{
    public static class LifetimeFactory
    {
        public static Lifetime BuildLifetime(
            EInitializationFlags initializationFlags = 
                EInitializationFlags.NO_ARGS_ALLOWED,
			
            Action setUp = null,
            Func<object[], bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null,
			
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<Lifetime>();
            
            return new Lifetime(
                initializationFlags,
                setUp,
                initialize,
                cleanup,
                tearDown,
                logger);
        }
        
        public static HierarchicalLifetime BuildHierarchicalLifetime(
            object target,
            IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool,
            
            ILifetimeable parentLifetime = null,
            EInitializationFlags initializationFlags = 
                EInitializationFlags.NO_ARGS_ALLOWED
                | EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE
                | EInitializationFlags.INITIALIZE_CHILDREN_ON_INITIALIZE
                | EInitializationFlags.INITIALIZE_WITH_PARENTS_ARGS,
            
            bool buildHierarchyNode = true,
			
            Action setUp = null,
            Func<object[], bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null,
			
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<Lifetime>();

            IHierarchyNode<ILifetimeable> hierarchyNode = null;
            
            if (buildHierarchyNode)
                hierarchyNode = new HierarchyNode<ILifetimeable>(
                new List<IReadOnlyHierarchyNode<ILifetimeable>>());
            
            var result = new HierarchicalLifetime(
                target,
                bufferPool,
                
                parentLifetime,
                initializationFlags,
                
                hierarchyNode,
                
                setUp,
                initialize,
                cleanup,
                tearDown,
                
                logger);
            
            if (buildHierarchyNode)
                hierarchyNode.Contents = result;

            return result;
        }
    }
}