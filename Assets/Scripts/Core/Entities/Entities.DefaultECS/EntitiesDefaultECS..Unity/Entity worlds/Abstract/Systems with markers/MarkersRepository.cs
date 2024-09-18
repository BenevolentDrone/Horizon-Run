#if USE_PROFILING_MARKERS
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using Unity.Profiling;

namespace HereticalSolutions.Entities
{
    public static class MarkersRepository
    {
        public static readonly IRepository<string, ProfilerMarker> Markers = RepositoriesFactory.BuildDictionaryRepository<string, ProfilerMarker>();
    }
}
#endif