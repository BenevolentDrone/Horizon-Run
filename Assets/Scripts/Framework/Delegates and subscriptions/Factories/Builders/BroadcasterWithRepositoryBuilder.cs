using System;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class BroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcasterRepository;

        private readonly ILoggerResolver loggerResolver;

        public BroadcasterWithRepositoryBuilder(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;

            broadcasterRepository = RepositoryFactory.BuildDictionaryRepository<Type, object>();
        }

        public BroadcasterWithRepositoryBuilder Add<TValue>()
        {
            broadcasterRepository.Add(
                typeof(TValue),
                BroadcasterFactory.BuildBroadcasterGeneric<TValue>(loggerResolver));

            return this;
        }

        public BroadcasterWithRepository BuildBroadcasterWithRepository()
        {
            return BroadcasterFactory.BuildBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }

        public ConcurrentBroadcasterWithRepository BuildConcurrentBroadcasterWithRepository()
        {
            return BroadcasterFactory.BuildConcurrentBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }
    }
}