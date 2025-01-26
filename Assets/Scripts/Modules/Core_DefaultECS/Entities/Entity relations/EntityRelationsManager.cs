/*
using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;


using TResource = HereticalSolutions.Relations.IReadOnlyDirectedNamedGraphNode<DefaultEcs.Entity>;

using THandle = System.UInt16; //ushort


namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class EntityRelationsManager
        : ManagedTypeResourceManagerWithPool<TResource, THandle>
    {
        public EntityRelationsManager(
            IRepository<THandle, TResource> resourceRepository,
            Queue<THandle> freeHandles,
            Func<THandle, THandle> newHandleAllocationDelegate,
            IPool<TResource> resourcePool,
            THandle uninitializedHandle = default(THandle),
            ILogger logger)
            : base(
                resourceRepository,
                freeHandles,
                newHandleAllocationDelegate,
                resourcePool,
                uninitializedHandle,
                logger)
        {
        }
    }
}
*/