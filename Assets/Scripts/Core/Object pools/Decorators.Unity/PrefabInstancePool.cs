using UnityEngine;

namespace HereticalSolutions.Pools.Decorators
{
	public class PrefabInstancePool : ADecoratorPool<GameObject>
	{
		private readonly GameObject prefab;

		public GameObject Prefab { get => prefab; }

		public PrefabInstancePool(
			IPool<GameObject> innerPool,
			GameObject prefab)
			: base(innerPool)
		{
			this.prefab = prefab;
		}
	}
}