using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Spawners")]
	public struct InvokeSubSpawnersComponent
	{
		public Entity[] SubSpawners;
	}
}