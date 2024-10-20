using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using Cinemachine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[ViewComponent]
	public class SampleVirtualCameraViewComponent : AMonoViewComponent
	{
		public CinemachineVirtualCamera VirtualCamera;
	}
}