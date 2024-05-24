using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public class DefaultECSEntityListManager
		: IEntityListManager<ushort, List<Entity>>
	{
		private readonly Queue<ushort> freeListHandles;

		private readonly IRepository<ushort, List<Entity>> entityListRepository;

		private readonly ILogger logger;
		
		private ushort nextHandleToAllocate;

		public DefaultECSEntityListManager(
			Queue<ushort> freeListHandles,
			IRepository<ushort, List<Entity>> entityListRepository,
			ILogger logger = null)
		{
			this.freeListHandles = freeListHandles;

			this.entityListRepository = entityListRepository;

			this.logger = logger;

			nextHandleToAllocate = 1;
		}

		public bool HasList(ushort listHandle)
		{
			if (listHandle == 0)
				return false;

			return entityListRepository.Has(listHandle);
		}

		public List<Entity> GetList(ushort listHandle)
		{
			if (listHandle == 0)
				throw new Exception(
					logger.TryFormat<DefaultECSEntityListManager>(
						$"INVALID LIST HANDLE {listHandle}"));
			
			if (!entityListRepository.TryGet(
				listHandle,
				out var entityList))
			{
				return null;
			}

			return entityList;
		}

		public void CreateList(
			out ushort listHandle,
			out List<Entity> entityList)
		{
			if (freeListHandles.Count > 0)
			{
				listHandle = freeListHandles.Dequeue();
			}
			else
			{
				listHandle = nextHandleToAllocate++;
			}

			entityList = new List<Entity>();

			entityListRepository.Add(
				listHandle,
				entityList);
			
			logger?.Log<DefaultECSEntityListManager>(
				$"CREATED LIST {listHandle}");
		}

		public void RemoveList(ushort listHandle)
		{
			if (listHandle == 0)
				return;

			if (!entityListRepository.TryRemove(listHandle))
			{
				return;
			}

			freeListHandles.Enqueue(listHandle);
			
			logger?.Log<DefaultECSEntityListManager>(
				$"REMOVED LIST {listHandle}");
		}
	}
}