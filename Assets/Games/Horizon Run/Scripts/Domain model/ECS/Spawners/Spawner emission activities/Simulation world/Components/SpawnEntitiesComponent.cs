using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Spawners")]
	public struct SpawnEntitiesComponent
	{
		public string PrototypeID;

		public int Amount;
	}
}