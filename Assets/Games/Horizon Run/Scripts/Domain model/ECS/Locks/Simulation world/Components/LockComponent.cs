using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Locks")]
	public struct LockComponent
	{
		public Entity PreviousLink;

		public Entity NextLink;
	}
}