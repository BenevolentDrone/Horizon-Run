using HereticalSolutions.Entities;

using HereticalSolutions.Logging;


using TWorld = DefaultEcs.World;

using TEntity = DefaultEcs.Entity;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class EventEntityWorldController
		: IEntityWorldController<TWorld, TEntity>
	{
		private readonly ILogger logger;

		public EventEntityWorldController(
			TWorld world,
			ILogger logger = null)
		{
			World = world;

			this.logger = logger;
		}

		#region IWorldController

		public TWorld World { get; private set; }


		public bool TrySpawnEntity(
			out TEntity entity)
		{
			entity = World.CreateEntity();

			return true;
		}

		public bool TrySpawnAndResolveEntity(
			object source,
			out TEntity entity)
		{
			//There's no use in resolving in registry world (for now)
			return TrySpawnEntity(
				out entity);
		}

		public bool DespawnEntity(
			TEntity entity)
		{
			if (entity == default)
				return false;

			if (entity.World != World)
			{
				logger?.LogError(
					GetType(),
					$"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

				return false;
			}

			if (entity.Has<DespawnComponent>())
				return false;

			entity.Set<DespawnComponent>();

			return true;
		}

		#endregion
	}
}