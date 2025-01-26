using System;
using System.Collections.Generic;

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
        #region Factory settings

        public const int DEFAULT_MESSAGE_POOL_CAPACITY = 32;

        public static AllocationCommandDescriptor MessagePoolInitialAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_MESSAGE_POOL_CAPACITY
            };

        public static AllocationCommandDescriptor MessagePoolAdditionalAllocationDescriptor =
            new AllocationCommandDescriptor
            {
                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                Amount = DEFAULT_MESSAGE_POOL_CAPACITY
            };

        #endregion

        private readonly IRepository<Type, IManagedPool<IMessage>> messagePoolRepository;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly ManagedPoolBuilder<IMessage> messagePoolBuilder;

        private readonly ILoggerResolver loggerResolver;


        public NonAllocMessageBusBuilder(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;

            messagePoolRepository = RepositoryFactory.BuildDictionaryRepository<Type, IManagedPool<IMessage>>();

            broadcasterBuilder = new NonAllocBroadcasterWithRepositoryBuilder(
                loggerResolver);
            
            messagePoolBuilder = new ManagedPoolBuilder<IMessage>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<IMessage>>());
        }

        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationFactory.ActivatorAllocationDelegate<IMessage, TMessage>;
            
            messagePoolBuilder.Initialize(
                valueAllocationDelegate,

                new Func<MetadataAllocationDescriptor>[]
                {
                    //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
                },
                MessagePoolInitialAllocationDescriptor,
                MessagePoolAdditionalAllocationDescriptor,
                null,
                null);
            
            IManagedPool<IMessage> messagePool = messagePoolBuilder.BuildPackedArrayManagedPool();
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public NonAllocMessageBus BuildNonAllocMessageBus()
        {
            ILogger logger = 
                loggerResolver?.GetLogger<NonAllocMessageBus>();

            return new NonAllocMessageBus(
                broadcasterBuilder.BuildNonAllocBroadcasterWithRepository(),
                messagePoolRepository,
                new Queue<IPoolElementFacade<IMessage>>(),
                logger);
        }

        public ConcurrentNonAllocMessageBus BuildConcurrentNonAllocMessageBus()
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentNonAllocMessageBus>();

            return new ConcurrentNonAllocMessageBus(
                broadcasterBuilder.BuildNonAllocBroadcasterWithRepository(),
                messagePoolRepository,
                new Queue<IPoolElementFacade<IMessage>>(),
                logger);
        }
    }
}