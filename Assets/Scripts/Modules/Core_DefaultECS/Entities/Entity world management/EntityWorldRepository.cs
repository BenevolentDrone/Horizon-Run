using System.Collections.Generic;
using System.Linq; //error CS1061: 'IEnumerable<World>' does not contain a definition for 'Contains'

using HereticalSolutions.Repositories;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;


using TWorldID = System.String;

using TWorld = DefaultEcs.World;

using TEntity = DefaultEcs.Entity;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class EntityWorldRepository
		: IEntityWorldRepository<
			TWorldID,
			TWorld,
			TEntity>
	{
		private readonly IRepository<TWorldID, TWorld> worldRepository;

		private readonly IRepository<TWorld, IEntityWorldController<TWorld, TEntity>> worldControllerRepository;

		private readonly ILogger logger;

		public EntityWorldRepository(
			IRepository<TWorldID, TWorld> worldRepository,
			IRepository<TWorld, IEntityWorldController<TWorld, TEntity>> worldControllerRepository,
			ILogger logger)
		{
			this.worldRepository = worldRepository;

			this.worldControllerRepository = worldControllerRepository;

			this.logger = logger;
		}

		#region IEntityWorldRepository

		#region IReadOnlyEntityWorldRepository

		public bool HasWorld(
			TWorldID worldID)
		{
			return worldRepository.Has(worldID);
		}

		public bool HasWorld(
			TWorld world)
		{
			return worldControllerRepository.Has(world);
		}

		public bool TryGetWorld(
			TWorldID worldID,
			out TWorld world)
		{
			return worldRepository.TryGet(
				worldID,
				out world);
		}

		public bool TryGetEntityWorldController(
			TWorldID worldID,
			out IEntityWorldController<TWorld, TEntity> entityWorldController)
		{
			if (!TryGetWorld(
				worldID,
				out var world))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD REGISTERED BY ID {worldID}");

				entityWorldController = default;

				return false;
			}

			if (!worldControllerRepository.TryGet(
				world,
				out entityWorldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER REGISTERED BY ID {worldID}");

				return false;
			}

			return true;
		}

		public bool TryGetEntityWorldController(
			TWorld world,
			out IEntityWorldController<TWorld, TEntity> entityWorldController)
		{
			if (!worldRepository.Values.Contains(world))
			{
				logger?.LogError(
					GetType(),
					$"WORLD {world} NOT REGISTERED");

				entityWorldController = default;

				return false;
			}

			if (!worldControllerRepository.TryGet(
				world,
				out entityWorldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER REGISTERED FOR THE WORLD {world.ToString()}");

				return false;
			}

			return true;
		}

		public IEnumerable<TWorldID> AllWorldIDs { get => worldRepository.Keys; }

		public IEnumerable<TWorld> AllWorlds { get => worldRepository.Values;}

		#endregion

		public bool AddWorld(
			TWorldID worldID,
			IEntityWorldController<TWorld, TEntity> worldController)
		{
			var world = worldController.World;

			bool success1 = worldRepository.TryAdd(
				worldID,
				world);

			bool success2 = worldControllerRepository.TryAdd(
				world,
				worldController);

			return success1 && success2;
		}


		public bool RemoveWorld(TWorldID worldID)
		{
			if (!worldRepository.TryGet(
				worldID,
				out var world))
			{
				return false;
			}

			bool success1 = worldRepository.TryRemove(
				worldID);

			bool success2 = worldControllerRepository.TryRemove(
				world);

			return success1 && success2;
		}

		public bool RemoveWorld(TWorld world)
		{
			TWorldID worldID = string.Empty;

			foreach (var key in worldRepository.Keys)
			{
				if (worldRepository.Get(key) == world)
				{
					worldID = key;

					break;
				}
			}

			if (string.IsNullOrEmpty(worldID))
			{
				return false;
			}

			bool success1 = worldRepository.TryRemove(
				worldID);

			bool success2 = worldControllerRepository.TryRemove(
				world);

			return success1 && success2;
		}

		#endregion
	}
}