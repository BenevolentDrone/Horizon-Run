using System;
using System.Collections.Generic;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement
{
    public static class LifetimeHelpers
    {
        public static void CreateAndSynchronizeDependencyLifetime(
            object target,
            ILifetimeable parentLifetime,
            IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool,
            ILoggerResolver loggerResolver,

            EInitializationFlags initializationFlags = 
                EInitializationFlags.NO_ARGS_ALLOWED
                | EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE
                | EInitializationFlags.INITIALIZE_CHILDREN_ON_INITIALIZE
                | EInitializationFlags.INITIALIZE_WITH_PARENTS_ARGS,
            
            Action setUp = null,
            Func<object[], bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null)
        {
            var dependencyLifetime = LifetimeFactory.BuildHierarchicalLifetime(
                target,
                bufferPool,
                loggerResolver,

                parentLifetime,
                initializationFlags,
                
                setUp: setUp,
                initialize: initialize,
                cleanup: cleanup,
                tearDown: tearDown);
            
            dependencyLifetime.SetUp();

            dependencyLifetime.Initialize();
        }
    }
}