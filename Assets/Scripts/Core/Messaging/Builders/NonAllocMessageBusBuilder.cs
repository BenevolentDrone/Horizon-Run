using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Factories
{
    public class NonAllocMessageBusBuilder
    {
        private const int DEFAULT_MESSAGE_POOL_CAPACITY = 16;
        
        private readonly IObjectRepository messagePoolRepository;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly ManagedPoolBuilder<IMessage> messagePoolBuilder;

        private readonly ManagedPoolBuilder<IPoolElementFacade<IMessage>> mailboxPoolBuilder;

        private readonly ILoggerResolver loggerResolver;


        public NonAllocMessageBusBuilder(
            ILoggerResolver loggerResolver = null)
        {
            this.loggerResolver = loggerResolver;

            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new NonAllocBroadcasterWithRepositoryBuilder(
                loggerResolver);
            
            messagePoolBuilder = new ManagedPoolBuilder<IMessage>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<IMessage>>());
            
            mailboxPoolBuilder = new ManagedPoolBuilder<IPoolElementFacade<IMessage>>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<IPoolElementFacade<IMessage>>>());
        }

        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;
            
            messagePoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
                null,
                null);
            
            IManagedPool<IMessage> messagePool = messagePoolBuilder.BuildPackedArrayManagedPool();
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public NonAllocMessageBus Build()
        {
            Func<IPoolElementFacade<IMessage>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IPoolElementFacade<IMessage>>;
            
            mailboxPoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor
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
                null,
                null);

            var mailbox = mailboxPoolBuilder.BuildPackedArrayManagedPool();
            
            ILogger logger = 
                loggerResolver?.GetLogger<NonAllocMessageBus>()
                ?? null;

            var mailboxContents =
                mailbox as IDynamicArray<IPoolElementFacade<IPoolElementFacade<IMessage>>>;
            
            return new NonAllocMessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                mailbox,
                mailboxContents,
                logger);
        }
    }
}