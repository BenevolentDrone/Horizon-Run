using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [Serializable]
    public class VFXElementSettings
    {
        public string Name;

        public GameObject Prefab;
        
        public float Duration;

        
        public AllocationCommandDescriptor Initial;

        public AllocationCommandDescriptor Additional;
    }
}