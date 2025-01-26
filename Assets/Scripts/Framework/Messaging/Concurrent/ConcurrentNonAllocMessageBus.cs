using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Delegates;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging
{
	public class ConcurrentNonAllocMessageBus
		: INonAllocMessageSender,
		  INonAllocMessageSubscribable
	{
		private readonly IPublisherSingleArg broadcaster;

		private readonly IReadOnlyRepository<Type, IManagedPool<IMessage>> messagePoolRepository;

		private readonly Queue<IPoolElementFacade<IMessage>> mailbox;

		private readonly ILogger logger;


		private readonly object mailboxLock;

		private readonly object poolLock;

		public ConcurrentNonAllocMessageBus(
			IPublisherSingleArg broadcaster,
			IReadOnlyRepository<Type, IManagedPool<IMessage>> messagePoolRepository,
			Queue<IPoolElementFacade<IMessage>> mailbox,
			ILogger logger)
		{
			this.broadcaster = broadcaster;

			this.messagePoolRepository = messagePoolRepository;

			this.mailbox = mailbox;

			this.logger = logger;


			mailboxLock = new object();

			poolLock = new object();
		}

		#region INonAllocMessageSender

		#region Pop

		public INonAllocMessageSender PopMessage<TMessage>(
			out IPoolElementFacade<IMessage> messageElementFacade)
			where TMessage : IMessage
		{
			lock (poolLock)
			{
				if (!messagePoolRepository.TryGet(
					typeof(TMessage),
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(TMessage)}"));
				}

				messageElementFacade = messagePool.Pop();
			}

			return this;
		}

		public INonAllocMessageSender PopMessage(
			Type messageType,
			out IPoolElementFacade<IMessage> messageElementFacade)
		{
			lock (poolLock)
			{
				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(messageType)}"));
				}

				messageElementFacade = messagePool.Pop();
			}

			return this;
		}

		#endregion

		#region Write

		public INonAllocMessageSender Write<TMessage>(
			IPoolElementFacade<IMessage> messageElementFacade,
			object[] args) where TMessage : IMessage
		{
			if (messageElementFacade == null || messageElementFacade.Value == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID MESSAGE"));
			}

			messageElementFacade.Value.Write(args);

			return this;
		}

		public INonAllocMessageSender Write(
			IPoolElementFacade<IMessage> messageElementFacade,
			object[] args)
		{
			if (messageElementFacade == null || messageElementFacade.Value == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID MESSAGE"));
			}

			messageElementFacade.Value.Write(args);

			return this;
		}

		#endregion

		#region Send

		public void PutIntoMailbox<TMessage>(
			IPoolElementFacade<IMessage> messageElementFacade)
			where TMessage : IMessage
		{
			lock (mailboxLock)
			{
				mailbox.Enqueue(messageElementFacade);
			}
		}

		public void PutIntoMailbox(
			IPoolElementFacade<IMessage> messageElementFacade)
		{
			lock (mailboxLock)
			{
				mailbox.Enqueue(messageElementFacade);
			}
		}

		public void SendImmediately<TMessage>(
			IPoolElementFacade<IMessage> messageElementFacade)
			where TMessage : IMessage
		{
			broadcaster.Publish<TMessage>(
				(TMessage)messageElementFacade.Value);

			PushMessageToPool<TMessage>(
				messageElementFacade);
		}

		public void SendImmediately(
			IPoolElementFacade<IMessage> messageElementFacade)
		{
			broadcaster.Publish(
				messageElementFacade.Value.GetType(),
				messageElementFacade);

			PushMessageToPool(
				messageElementFacade);
		}

		#endregion

		#region Deliver

		public void DeliverMessagesInMailbox()
		{
			int messagesToReceive = 0;

			lock (mailboxLock)
			{
				messagesToReceive = mailbox.Count;
			}

			for (int i = 0; i < messagesToReceive; i++)
			{
				IPoolElementFacade<IMessage> message;

				lock (mailboxLock)
				{
					message = mailbox.Dequeue();
				}

				SendImmediately(message);
			}
		}

		#endregion

		private void PushMessageToPool<TMessage>(
			IPoolElementFacade<IMessage> messageElementFacade)
			where TMessage : IMessage
		{
			lock (poolLock)
			{
				var messageType = typeof(TMessage);

				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(TMessage)}"));
				}

				messagePool.Push(messageElementFacade);
			}
		}

		private void PushMessageToPool(
			IPoolElementFacade<IMessage> messageElementFacade)
		{
			lock (poolLock)
			{
				var messageType = messageElementFacade.Value.GetType();

				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}"));
				}

				messagePool.Push(messageElementFacade);
			}
		}

		#endregion

		#region IMessageReceiver

		public void SubscribeTo<TMessage>(
			INonAllocSubscription subscription)
			where TMessage : IMessage
		{
			var broadcasterAsSubscribable = broadcaster as INonAllocSubscribable;

			broadcasterAsSubscribable?.Subscribe(
				subscription);
		}

		public void SubscribeTo(
			Type messageType,
			INonAllocSubscription subscription)
		{
			var broadcasterAsSubscribable = broadcaster as INonAllocSubscribable;

			broadcasterAsSubscribable?.Subscribe(
				subscription);
		}

		public void UnsubscribeFrom<TMessage>(
			INonAllocSubscription subscription)
			where TMessage : IMessage
		{
			var broadcasterAsSubscribable = broadcaster as INonAllocSubscribable;

			broadcasterAsSubscribable?.Unsubscribe(
				subscription);
		}

		public void UnsubscribeFrom(
			Type messageType,
			INonAllocSubscription subscription)
		{
			var broadcasterAsSubscribable = broadcaster as INonAllocSubscribable;

			broadcasterAsSubscribable?.Unsubscribe(
				subscription);
		}

		#endregion
	}
}