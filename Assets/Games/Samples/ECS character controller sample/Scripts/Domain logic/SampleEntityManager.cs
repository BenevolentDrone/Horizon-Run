using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using World = DefaultEcs.World;

using Entity = DefaultEcs.Entity;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleEntityManager : DefaultECSEntityManager<Guid>
	{
		public SampleEntityManager(
			Func<Guid> allocateIDDelegate,
			IRepository<Guid, Entity> registryEntitiesRepository,
			IReadOnlyEntityWorldsRepository<World, IDefaultECSEntityWorldController> entityWorldsRepository,
			IReadOnlyList<World> childEntityWorlds,
			ILogger logger = null)
			: base (
				allocateIDDelegate,
				registryEntitiesRepository,
				entityWorldsRepository,
				childEntityWorlds,
				logger)
		{}
	}
}