#if USE_PROFILING_SAMPLERS
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using UnityEngine.Profiling;

namespace HereticalSolutions.Entities
{
    public static class SamplersRepository
    {
        public static readonly IRepository<string, CustomSampler> Samplers = RepositoriesFactory.BuildDictionaryRepository<string, CustomSampler>();
    }
}
#endif