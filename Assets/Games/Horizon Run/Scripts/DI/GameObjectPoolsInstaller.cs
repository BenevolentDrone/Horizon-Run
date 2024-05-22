using HereticalSolutions.Pools;

using HereticalSolutions.HorizonRun.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
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
			var prefabsPool = GameObjectPoolsFactory.BuildPool(
				Container,
				prefabsPoolsSettings,
				poolParentTransform,
				loggerResolver);

			Container
				.Bind<INonAllocDecoratedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}