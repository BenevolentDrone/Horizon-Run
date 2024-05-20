using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	[Serializable]
	public class SampleGameObjectVariantSettings
	{
		public float Chance;

		public GameObject Prefab;

		public AllocationCommandDescriptor Initial;

		public AllocationCommandDescriptor Additional;
	}
}