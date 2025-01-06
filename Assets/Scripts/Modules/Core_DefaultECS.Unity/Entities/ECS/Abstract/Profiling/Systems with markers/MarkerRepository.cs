#if USE_PROFILING_MARKERS
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using Unity.Profiling;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public static class MarkerRepository
    {
        public static readonly IRepository<string, ProfilerMarker> Markers =
            RepositoriesFactory.BuildDictionaryRepository<string, ProfilerMarker>();
    }
}
#endif