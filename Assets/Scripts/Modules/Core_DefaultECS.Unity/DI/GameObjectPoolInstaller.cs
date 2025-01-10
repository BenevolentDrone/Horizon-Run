using HereticalSolutions.Pools;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class GameObjectPoolInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private GameObjectPoolSettings prefabsPoolSettings;

		[SerializeField]
		private Transform poolParentTransform;

		public override void InstallBindings()
		{
			var prefabsPool = GameObjectPoolFactory.BuildPool(
				Container,
				prefabsPoolSettings,
				poolParentTransform,
				loggerResolver);

			Container
				.Bind<IManagedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}