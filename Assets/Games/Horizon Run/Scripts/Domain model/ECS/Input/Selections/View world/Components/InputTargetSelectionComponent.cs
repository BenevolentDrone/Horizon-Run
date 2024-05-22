using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	[Component("View world/Selection")]
	public struct InputTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}