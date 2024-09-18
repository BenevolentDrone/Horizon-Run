using System;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class NonAllocBroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        private readonly ILoggerResolver loggerResolver;

        public NonAllocBroadcasterWithRepositoryBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(
                typeof(TBroadcaster),
                BroadcastersFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public NonAllocBroadcasterWithRepository Build()
        {
            return BroadcastersFactory.BuildNonAllocBroadcasterWithRepository(
                broadcastersRepository,
                loggerResolver);
        }
    }
}