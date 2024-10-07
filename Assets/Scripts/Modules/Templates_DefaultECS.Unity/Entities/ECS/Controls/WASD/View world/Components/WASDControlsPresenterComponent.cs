using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Presenters")]
	public struct WASDControlsPresenterComponent
	{
		public Entity TargetEntity;
	}
}