using HereticalSolutions.Pools;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
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
			var prefabsPool = GameObjectPoolFactory.BuildPool(
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