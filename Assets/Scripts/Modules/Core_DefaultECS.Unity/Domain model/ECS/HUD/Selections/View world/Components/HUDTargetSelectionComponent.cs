using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Selection")]
	public struct HUDTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}