using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Selection")]
	public struct CameraTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}