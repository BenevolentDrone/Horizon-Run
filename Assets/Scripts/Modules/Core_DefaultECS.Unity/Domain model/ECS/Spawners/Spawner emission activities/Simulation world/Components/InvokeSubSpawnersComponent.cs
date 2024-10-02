using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Spawners")]
	public struct InvokeSubSpawnersComponent
	{
		public Entity[] SubSpawners;
	}
}