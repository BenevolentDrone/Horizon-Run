using DefaultEcs;

namespace HereticalSolutions.Entities
{
	[Component("Hierarchy")]
	public struct HierarchyComponent
	{
		public Entity Parent;

		public ushort ChildrenListHandle;
	}
}