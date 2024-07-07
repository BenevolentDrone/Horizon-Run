using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("Simulation world/Locks")]
	public struct LockComponent
	{
		public Entity PreviousLink;

		public Entity NextLink;
	}
}