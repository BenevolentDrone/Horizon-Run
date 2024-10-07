using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Spawners")]
	public struct SpawnEntitiesComponent
	{
		public string PrototypeID;

		public int Amount;
	}
}