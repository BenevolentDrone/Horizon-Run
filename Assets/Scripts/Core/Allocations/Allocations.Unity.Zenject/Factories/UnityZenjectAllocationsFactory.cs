using Zenject;

using UnityEngine;

namespace HereticalSolutions.Allocations.Factories
{
    public static class UnityZenjectAllocationsFactory
    {
        public static GameObject DIResolveAllocationDelegate(
            DiContainer container,
            GameObject prefab)
        {
            return container.InstantiatePrefab(prefab);
        }
		
        public static GameObject DIResolveOrInstantiateAllocationDelegate(
            DiContainer container,
            GameObject prefab)
        {
            return container != null
                ? container.InstantiatePrefab(prefab)
                : GameObject.Instantiate(prefab) as GameObject;
        }
    }
}