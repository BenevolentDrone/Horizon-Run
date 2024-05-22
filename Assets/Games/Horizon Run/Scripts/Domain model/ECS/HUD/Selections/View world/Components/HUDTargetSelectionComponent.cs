using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
	[Component("View world/Selection")]
	public struct HUDTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}