using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Spawners")]
	public struct DoNotEmitIfEntitiesPresentComponent
	{
		public string[] RelevantSpaceIDs;
	}
}