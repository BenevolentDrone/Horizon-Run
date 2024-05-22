using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
	public class CameraInstaller : MonoInstaller
	{
		[SerializeField]
		private Camera mainRenderCamera;

		public override void InstallBindings()
		{
			Container
				.Bind<Camera>()
				.WithId("Main render camera")
				.FromInstance(mainRenderCamera)
				.AsCached();
		}
	}
}