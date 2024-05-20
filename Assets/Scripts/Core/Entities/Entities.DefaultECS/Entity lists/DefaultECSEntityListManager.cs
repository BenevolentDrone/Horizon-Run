using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public class DefaultECSEntityListManager
		: IEntityListManager<int, List<Entity>>
	{
		private readonly ILogger logger;
		
		private int nextIDToAllocate;

		private Queue<int> freeListIDs;

		private IRepository<int, List<Entity>> entityListRepository;

		public DefaultECSEntityListManager(
			Queue<int> freeListIDs,
			IRepository<int, List<Entity>> entityListRepository,
			ILogger logger = null)
		{
			this.freeListIDs = freeListIDs;

			this.entityListRepository = entityListRepository;

			this.logger = logger;

			nextIDToAllocate = 1;
		}

		public bool HasList(int listID)
		{
			if (listID == 0)
				return false;
			
	        /*
	            throw new Exception(
		            logger.TryFormat<DefaultECSEntityListManager>(
			            $"INVALID LIST ID {listID}"));
            */
            
			return entityListRepository.Has(listID);
		}

		public List<Entity> GetList(int listID)
		{
			if (listID == 0)
				throw new Exception(
					logger.TryFormat<DefaultECSEntityListManager>(
						$"INVALID LIST ID {listID}"));
			
			if (!entityListRepository.TryGet(
				listID,
				out var entityList))
			{
				return null;
			}

			return entityList;
		}

		public void CreateList(
			out int listID,
			out List<Entity> entityList)
		{
			if (freeListIDs.Count > 0)
			{
				listID = freeListIDs.Dequeue();
			}
			else
			{
				listID = nextIDToAllocate++;
			}

			entityList = new List<Entity>();

			entityListRepository.Add(
				listID,
				entityList);
			
			logger?.Log<DefaultECSEntityListManager>(
				$"CREATED LIST {listID}");
		}

		public void RemoveList(int listID)
		{
			if (listID == 0)
				return;
			
			/*
				throw new Exception(
					logger.TryFormat<DefaultECSEntityListManager>(
						$"INVALID LIST ID {listID}"));
			*/
			
			if (!entityListRepository.TryRemove(listID))
			{
				return;
			}

			freeListIDs.Enqueue(listID);
			
			logger?.Log<DefaultECSEntityListManager>(
				$"REMOVED LIST {listID}");
		}
	}
}