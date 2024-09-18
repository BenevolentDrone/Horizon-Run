using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.Relations;

using HereticalSolutions.Logging;

using DefaultEcs;

//God damn it, Unity :(
//using HereticalSolutions.Entities;
//global using DefaultECSEntityListManager = IManagedTypeResourceManager<List<Entity>, ushort>; 

namespace HereticalSolutions.Entities
{
    public class DefaultECSEntityRelationsManager
        : ManagedTypeResourceManagerWithPool<IReadOnlyDirectedNamedGraphNode<Entity>, ushort>
    {
        public DefaultECSEntityRelationsManager(
            IRepository<ushort, IReadOnlyDirectedNamedGraphNode<Entity>> resourceRepository,
            Queue<ushort> freeHandles,
            Func<ushort, ushort> newHandleAllocationDelegate,
            IPool<IReadOnlyDirectedNamedGraphNode<Entity>> resourcePool,
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