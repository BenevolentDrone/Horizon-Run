using HereticalSolutions.Pools;

using HereticalSolutions.Templates.Universal.Unity.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Templates.Universal.Unity.DI
{
	public class GameObjectPoolsInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private GameObjectPoolSettings prefabsPoolsSettings;

		[SerializeField]
		private Transform poolParentTransform;

		public override void InstallBindings()
		{
			var prefabsPool = UniversalTemplateUnityGameObjectPoolsFactory.BuildPool(
				Container,
				prefabsPoolsSettings,
				poolParentTransform,
				loggerResolver);

			Container
				.Bind<IManagedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}