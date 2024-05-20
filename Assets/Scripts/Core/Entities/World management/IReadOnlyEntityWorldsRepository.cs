using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
	public interface IReadOnlyEntityWorldsRepository<TWorld, TWorldController>
	{
		bool HasWorld(string worldID);

		bool HasWorld(TWorld world);
		
		
		TWorld GetWorld(string worldID);

		
		TWorldController GetWorldController(string worldID);

		TWorldController GetWorldController(TWorld world);

		
		IEnumerable<string> AllWorldIDs { get; }

		IEnumerable<TWorld> AllWorlds { get; }
	}
}