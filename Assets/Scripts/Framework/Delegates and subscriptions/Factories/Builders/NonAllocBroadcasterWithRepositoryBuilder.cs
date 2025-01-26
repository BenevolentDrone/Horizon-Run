using System;

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
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;

            broadcasterRepository = RepositoryFactory.BuildDictionaryRepository<Type, object>();
        }

        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcasterRepository.Add(
                typeof(TBroadcaster),
                BroadcasterFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository()
        {
            return BroadcasterFactory.BuildNonAllocBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }

        public ConcurrentNonAllocBroadcasterWithRepository BuildConcurrentNonAllocBroadcasterWithRepository()
        {
            return BroadcasterFactory.BuildConcurrentNonAllocBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }

        public AsyncBroadcasterWithRepository BuildAsyncBroadcasterWithRepository()
        {
            return BroadcasterFactory.BuildAsyncBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }
    }
}