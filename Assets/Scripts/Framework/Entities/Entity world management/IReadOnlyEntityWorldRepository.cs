using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
	public interface IReadOnlyEntityWorldRepository<TWorldID, TWorld, TEntity>
	{
		bool HasWorld(
			TWorldID worldID);

		bool HasWorld(
			TWorld world);
		
		
		bool TryGetWorld(
			TWorldID worldID,
			out TWorld world);

		
		bool TryGetEntityWorldController(
			TWorldID worldID,
			out IEntityWorldController<TWorld, TEntity> entityWorldController);

		bool TryGetEntityWorldController(
			TWorld world,
			out IEntityWorldController<TWorld, TEntity> entityWorldController);

		
		IEnumerable<TWorldID> AllWorldIDs { get; }

		IEnumerable<TWorld> AllWorlds { get; }
	}
}