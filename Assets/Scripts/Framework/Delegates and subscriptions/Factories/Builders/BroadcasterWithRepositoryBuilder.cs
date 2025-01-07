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
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            broadcasterRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public BroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcasterRepository.Add(
                typeof(TBroadcaster),
                BroadcastersFactory.BuildBroadcasterGeneric<TBroadcaster>(loggerResolver));

            return this;
        }

        public BroadcasterWithRepository Build()
        {
            return BroadcastersFactory.BuildBroadcasterWithRepository(
                broadcasterRepository,
                loggerResolver);
        }
    }
}