using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[Component("View world/Presenters")]
	public struct VirtualCameraPresenterComponent
	{
		public Entity TargetEntity;
	}
}