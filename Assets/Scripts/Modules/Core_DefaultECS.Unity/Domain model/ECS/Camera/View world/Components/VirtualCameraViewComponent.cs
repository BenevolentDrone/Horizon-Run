using HereticalSolutions.Entities;

using Cinemachine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[ViewComponent]
	public class VirtualCameraViewComponent : AMonoViewComponent
	{
        public CinemachineVirtualCamera VirtualCamera;
	}
}