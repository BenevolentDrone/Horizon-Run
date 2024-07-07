using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Spawners")]
	public struct SpawnEntitiesComponent
	{
		public string PrototypeID;

		public int Amount;
	}
}