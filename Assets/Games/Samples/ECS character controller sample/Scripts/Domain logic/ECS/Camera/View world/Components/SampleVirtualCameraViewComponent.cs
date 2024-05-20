using HereticalSolutions.Entities;

using Cinemachine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[ViewComponent]
	public class SampleVirtualCameraViewComponent : AMonoViewComponent
	{
		public CinemachineVirtualCamera VirtualCamera;
	}
}