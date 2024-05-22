using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Spawners")]
	public struct DoNotEmitIfEntitiesPresentComponent
	{
		public string[] RelevantSpaceIDs;
	}
}