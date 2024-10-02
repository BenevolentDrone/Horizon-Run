using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Selection")]
	public struct InputTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}