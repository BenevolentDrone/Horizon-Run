using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Factories
{
    public class NonAllocMessageBusBuilder
    {
        private const int DEFAULT_MESSAGE_POOL_CAPACITY = 16;
        
        private readonly IObjectRepository messagePoolRepository;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly ILoggerResolver loggerResolver;


        public NonAllocMessageBusBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new NonAllocBroadcasterWithRepositoryBuilder(
                loggerResolver);
        }

        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;
            
            INonAllocDecoratedPool<IMessage> messagePool = PoolsFactory.BuildResizableNonAllocPool<IMessage>(
                valueAllocationDelegate,
                true,

                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_CAPACITY
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                loggerResolver);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public NonAllocMessageBus Build()
        {
            Func<IPoolElement<IMessage>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IPoolElement<IMessage>>;
            
            INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox = PoolsFactory.BuildResizableNonAllocPool<IPoolElement<IMessage>>(
                valueAllocationDelegate,
                false,

                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_CAPACITY
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },
                loggerResolver);
            
            var mailboxContents = ((IModifiable<INonAllocPool<IPoolElement<IMessage>>>)mailbox).Contents;
            
            var mailboxContentAsIndexable = (IIndexable<IPoolElement<IPoolElement<IMessage>>>)mailboxContents;
            
            ILogger logger = 
                loggerResolver?.GetLogger<NonAllocMessageBus>()
                ?? null;

            return new NonAllocMessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                mailbox,
                mailboxContentAsIndexable,
                logger);
        }
    }
}