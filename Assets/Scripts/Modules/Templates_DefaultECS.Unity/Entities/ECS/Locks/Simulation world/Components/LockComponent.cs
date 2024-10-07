using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("Simulation world/Locks")]
	public struct LockComponent
	{
		public Entity PreviousLink;

		public Entity NextLink;
	}
}