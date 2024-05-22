using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	[Component("View world/Selection")]
	public struct CameraTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}