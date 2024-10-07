using DefaultEcs.System;

#if USE_PROFILING_CUSTOM_MARKERS
using HereticalSolutions.Profiling;
#endif

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[IgnoreProfiling]
	public class SystemWithNamedCustomMarker<TDelta>
		: ISystem<TDelta>
	{
		private readonly ISystem<TDelta> innerSystem;

#if USE_PROFILING_CUSTOM_MARKERS
        private readonly ProfilingMarker marker;
#endif

		public SystemWithNamedCustomMarker(
			string markerName,
			ISystem<TDelta> innerSystem)
		{
			this.innerSystem = innerSystem;

#if USE_PROFILING_CUSTOM_MARKERS
            marker = ProfilingManager.AllocateMarker(markerName);
#endif
		}

		public bool IsEnabled { get; set; } = true;

		public void Update(TDelta delta)
		{
#if USE_PROFILING_CUSTOM_MARKERS
            marker.Start();
#endif
			innerSystem.Update(delta);
#if USE_PROFILING_CUSTOM_MARKERS
            marker.Stop();
#endif
		}

		public void Dispose()
		{
		}
	}
}