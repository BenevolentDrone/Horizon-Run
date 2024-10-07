using DefaultEcs.System;

#if USE_PROFILING_MARKERS
using Unity.Profiling;
#endif

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [IgnoreProfiling]
    public class SystemWithMarker<TDelta>
        : ISystem<TDelta>
    {
        private readonly ISystem<TDelta> innerSystem;

#if USE_PROFILING_MARKERS
        private readonly ProfilerMarker marker;
#endif

        public SystemWithMarker(
            ISystem<TDelta> innerSystem)
        {
            this.innerSystem = innerSystem;
      
#if USE_PROFILING_MARKERS
            string markerName = innerSystem.GetType().Name;
            
            marker = new ProfilerMarker(markerName);
            
            if (!MarkersRepository.Markers.Has(markerName))
            {
                MarkersRepository.Markers.Add(
                    markerName,
                    marker);
            }
#endif
        }

        public bool IsEnabled { get; set; } = true;

        public void Update(TDelta delta)
        {
#if USE_PROFILING_MARKERS
            using (marker.Auto()) //To ensure it stays in release build
            {
            //marker.Begin();
#endif
                innerSystem.Update(delta);
#if USE_PROFILING_MARKERS
            }
            //marker.End();
#endif
        }

        public void Dispose()
        {
        }
    }
}