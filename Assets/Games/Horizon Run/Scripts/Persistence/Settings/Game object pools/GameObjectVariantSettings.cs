using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Serializable]
	public class GameObjectVariantSettings
	{
		public float Chance;

		public GameObject Prefab;

		public AllocationCommandDescriptor Initial;

		public AllocationCommandDescriptor Additional;
	}
}