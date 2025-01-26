using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Pools.Factories
{
	public static class UnityDecoratorPoolFactory
	{
		public static GameObjectPool BuildGameObjectPool(
			IPool<GameObject> innerPool,
			Transform parentTransform = null)
		{
			return new GameObjectPool(
				innerPool,
				parentTransform);
		}

		public static PrefabInstancePool BuildPrefabInstancePool(
			IPool<GameObject> innerPool,
			GameObject prefab)
		{
			return new PrefabInstancePool(
				innerPool,
				prefab);
		}

		public static GameObjectManagedPool BuildGameObjectManagedPool(
			IManagedPool<GameObject> innerPool,
			ILoggerResolver loggerResolver,
			Transform parentTransform = null)
		{
			ILogger logger = loggerResolver?.GetLogger<GameObjectManagedPool>();

			return new GameObjectManagedPool(
				innerPool,
				parentTransform,
				logger);
		}

		public static PrefabInstanceManagedPool BuildPrefabInstanceManagedPool(
			IManagedPool<GameObject> innerPool,
			GameObject prefab,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<PrefabInstanceManagedPool>();

			return new PrefabInstanceManagedPool(
				innerPool,
				prefab,
				logger);
		}
	}
}