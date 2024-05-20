using System;

namespace HereticalSolutions.Samples.PoolWithAddressVariantAndTimerSample
{
    [Serializable]
    public class SamplePoolSettings
    {
        public string ID;

        public SampleElementSettings[] Elements;
    }
}