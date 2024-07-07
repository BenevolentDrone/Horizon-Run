using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[Component("View world/Selection")]
	public struct InputTargetSelectionComponent
	{
		public Entity TargetEntity;
	}
}