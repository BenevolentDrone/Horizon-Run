using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;


using TResource = System.Collections.Generic.List<DefaultEcs.Entity>;

using THandle = System.UInt16; //ushort


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class EntityListManager
		: ManagedTypeResourceManagerWithPool<TResource, THandle>
	{
		public EntityListManager(
			IRepository<THandle, TResource> resourceRepository,
			Queue<THandle> freeHandles,
            Func<THandle, THandle> newHandleAllocationDelegate,
            IPool<TResource> resourcePool,
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