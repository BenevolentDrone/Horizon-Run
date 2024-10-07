#if USE_PROFILING_SAMPLERS
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using UnityEngine.Profiling;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public static class SamplersRepository
    {
        public static readonly IRepository<string, CustomSampler> Samplers =
            RepositoriesFactory.BuildDictionaryRepository<string, CustomSampler>();
    }
}
#endif