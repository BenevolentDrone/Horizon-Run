using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

using DefaultEcs;

//God damn it, Unity :(
//using HereticalSolutions.Entities;
//global using DefaultECSEntityListManager = IManagedTypeResourceManager<List<Entity>, ushort>; 

namespace HereticalSolutions.Entities
{
	public class DefaultECSEntityListManager
		: ManagedTypeResourceManagerWithPool<List<Entity>, ushort>
	{
		public DefaultECSEntityListManager(
			IRepository<ushort, List<Entity>> resourceRepository,
			Queue<ushort> freeHandles,
            Func<ushort, ushort> newHandleAllocationDelegate,
            IPool<List<Entity>> resourcePool,
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