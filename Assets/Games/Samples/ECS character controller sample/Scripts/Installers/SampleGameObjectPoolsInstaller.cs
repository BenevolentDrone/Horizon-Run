using HereticalSolutions.Pools;

using HereticalSolutions.Samples.ECSCharacterControllerSample.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleGameObjectPoolsInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private SampleGameObjectPoolSettings prefabsPoolsSettings;

		public override void InstallBindings()
		{
			var prefabsPool = SampleGameObjectPoolsFactory.BuildPool(
				Container,
				prefabsPoolsSettings,
				null,
				loggerResolver);

			Container
				.Bind<INonAllocDecoratedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}