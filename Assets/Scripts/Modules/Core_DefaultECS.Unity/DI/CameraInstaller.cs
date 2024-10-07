using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
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