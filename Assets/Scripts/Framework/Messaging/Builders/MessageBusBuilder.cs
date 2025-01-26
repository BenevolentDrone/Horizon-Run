using System;
using System.Collections.Generic;

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
    public class MessageBusBuilder
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

        private readonly IRepository<Type, IPool<IMessage>> messagePoolRepository;

        private readonly BroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly ILoggerResolver loggerResolver;

        public MessageBusBuilder(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;

            messagePoolRepository = RepositoryFactory.BuildDictionaryRepository<Type, IPool<IMessage>>();

            broadcasterBuilder = new BroadcasterWithRepositoryBuilder(
                loggerResolver);
        }

        public MessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<IMessage, TMessage>;

            var initialAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = MessagePoolInitialAllocationDescriptor,

                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = MessagePoolAdditionalAllocationDescriptor,

                AllocationDelegate = valueAllocationDelegate
            };
            
            IPool<IMessage> messagePool = StackPoolFactory.BuildStackPool<IMessage>(
                initialAllocationCommand,
                additionalAllocationCommand,
                loggerResolver);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public MessageBus BuildMessageBus()
        {
            ILogger logger = 
                loggerResolver?.GetLogger<MessageBus>();

            return new MessageBus(
                broadcasterBuilder.BuildBroadcasterWithRepository(),
                messagePoolRepository,
                new Queue<IMessage>(),
                logger);
        }

        public ConcurrentMessageBus BuildConcurrentMessageBus()
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentMessageBus>();

            return new ConcurrentMessageBus(
                broadcasterBuilder.BuildBroadcasterWithRepository(),
                messagePoolRepository,
                new Queue<IMessage>(),
                logger);
        }
    }
}