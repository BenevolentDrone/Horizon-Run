using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("View world/Selection")]
	public struct HUDTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}