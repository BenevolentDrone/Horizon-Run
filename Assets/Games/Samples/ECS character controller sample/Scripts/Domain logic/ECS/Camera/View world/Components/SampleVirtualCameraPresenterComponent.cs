using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Component("View world/Presenters")]
	public struct SampleVirtualCameraPresenterComponent
	{
		public Entity TargetEntity;
	}
}