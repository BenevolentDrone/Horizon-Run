using HereticalSolutions.Pools;

using HereticalSolutions.Samples.ECSCharacterControllerSample.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleGameObjectPoolInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private SampleGameObjectPoolSettings prefabsPoolSettings;

		public override void InstallBindings()
		{
			var prefabsPool = SampleGameObjectPoolFactory.BuildPool(
				Container,
				prefabsPoolSettings,
				null,
				loggerResolver);

			Container
				.Bind<IManagedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}