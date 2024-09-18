using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Logging;

using DefaultEcs;

//God damn it, Unity :(
//using HereticalSolutions.Entities;
//global using DefaultECSEntityListManager = IManagedTypeResourceManager<List<Entity>, ushort>; 

namespace HereticalSolutions.Entities
{
    public class DefaultECSEntityHierarchyManager
        : ManagedTypeResourceManagerWithPool<IReadOnlyHierarchyNode<Entity>, ushort>
    {
        public DefaultECSEntityHierarchyManager(
            IRepository<ushort, IReadOnlyHierarchyNode<Entity>> resourceRepository,
            Queue<ushort> freeHandles,
            Func<ushort, ushort> newHandleAllocationDelegate,
            IPool<IReadOnlyHierarchyNode<Entity>> resourcePool,
            ILogger logger = null)
            : base(
                resourceRepository,
                freeHandles,
                newHandleAllocationDelegate,
                resourcePool,
                logger)
        {
        }
    }
}