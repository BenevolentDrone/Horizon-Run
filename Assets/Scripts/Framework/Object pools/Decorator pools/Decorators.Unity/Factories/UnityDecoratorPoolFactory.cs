using UnityEngine;

using HereticalSolutions.Pools.Decorators;

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
			Transform parentTransform = null)
		{
			return new GameObjectManagedPool(
				innerPool,
				parentTransform);
		}

		public static PrefabInstanceManagedPool BuildPrefabInstanceManagedPool(
			IManagedPool<GameObject> innerPool,
			GameObject prefab)
		{
			return new PrefabInstanceManagedPool(
				innerPool,
				prefab);
		}
	}
}