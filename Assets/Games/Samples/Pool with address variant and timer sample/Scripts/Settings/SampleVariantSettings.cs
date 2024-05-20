using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.Samples.PoolWithAddressVariantAndTimerSample
{
    [Serializable]
    public class SampleVariantSettings
    {
        public float Chance;

        public GameObject Prefab;
        
        public float Duration;

        public AllocationCommandDescriptor Initial;

        public AllocationCommandDescriptor Additional;
    }
}