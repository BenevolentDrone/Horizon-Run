using UnityEngine;

namespace HereticalSolutions.Pools.Decorators
{
	public class PrefabInstanceManagedPool : ADecoratorManagedPool<GameObject>
	{
		private readonly GameObject prefab;

		public GameObject Prefab { get => prefab; }

		public PrefabInstanceManagedPool(
			IManagedPool<GameObject> innerPool,
			GameObject prefab)
			: base(innerPool)
		{
			this.prefab = prefab;
		}
	}
}