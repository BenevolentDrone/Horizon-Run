using System;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class NonAllocBroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcasterRepository;

        private readonly ILoggerResolver loggerResolver;

        public NonAllocBroadcasterWithRepositoryBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            broadcasterRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcasterRepository.Add(
                typeof(TBroadcaster),
                BroadcastersFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public NonAllocBroadcasterWithRepository Build()
        {
            return BroadcastersFactory.BuildNonAllocBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }
    }
}