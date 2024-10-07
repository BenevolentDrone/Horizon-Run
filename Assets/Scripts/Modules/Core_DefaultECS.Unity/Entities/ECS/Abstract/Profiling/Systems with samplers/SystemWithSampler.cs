using DefaultEcs.System;

#if USE_PROFILING_SAMPLERS
using UnityEngine.Profiling;
#endif

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [IgnoreProfiling]
    public class SystemWithSampler<TDelta>
        : ISystem<TDelta>
    {
        private readonly ISystem<TDelta> innerSystem;

#if USE_PROFILING_SAMPLERS
        private readonly CustomSampler sampler;
#endif

        public SystemWithSampler(
            ISystem<TDelta> innerSystem)
        {
            this.innerSystem = innerSystem;
      
#if USE_PROFILING_SAMPLERS
            string samplerName = innerSystem.GetType().Name;

            sampler = CustomSampler.Create(samplerName);
            
            if (!SamplersRepository.Samplers.Has(samplerName))
            {
                SamplersRepository.Samplers.Add(
                    samplerName,
                    sampler);
            }
#endif
        }

        public bool IsEnabled { get; set; } = true;

        public void Update(TDelta delta)
        {
#if USE_PROFILING_SAMPLERS
            sampler.Begin();
#endif
            
            innerSystem.Update(delta);
            
#if USE_PROFILING_SAMPLERS
            sampler.End();
#endif
        }

        public void Dispose()
        {
        }
    }
}