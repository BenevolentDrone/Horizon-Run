using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.Samples.ResizableGameObjectPoolSample
{
    [Serializable]
    public class SamplePoolSettings
    {
        public GameObject Prefab;

        public AllocationCommandDescriptor Initial;

        public AllocationCommandDescriptor Additional;
    }
}