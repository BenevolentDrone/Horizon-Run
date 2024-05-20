using System;

using UnityEngine;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;
using HereticalSolutions.Messaging.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Samples.NonAllocatingMessageBusSample
{
	public class NonAllocatingMessageBusSampleBehaviour : MonoBehaviour
	{
		private INonAllocMessageSender messageBusAsSender;

		private INonAllocMessageReceiver messageBusAsReceiver;


		private ISubscription subscription;


		private readonly string messageText1 = "Generic message";

		private readonly string messageText2 = "Mailbox message";


		private object[] messageArgs;


		private ILoggerResolver loggerResolver;

		private ILogger logger;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(false)
				.AddOrWrap(
					LoggersFactoryUnity.BuildUnityDebugLogger())
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
						loggerBuilder.CurrentLogger))
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
						loggerBuilder.CurrentLogger))
				.ToggleLogSource(typeof(NonAllocatingMessageBusSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<NonAllocatingMessageBusSampleBehaviour>();

			#endregion

			#region Message bus

			var builder = new NonAllocMessageBusBuilder(loggerResolver);

			builder.AddMessageType<SampleMessage>();

			var messageBus = builder.Build();

			messageBusAsSender = (INonAllocMessageSender)messageBus;

			messageBusAsReceiver = (INonAllocMessageReceiver)messageBus;

			#endregion

			#region Subscription

			subscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<SampleMessage>(Print, loggerResolver);

			#endregion

			#region Message

			string messageArgument = "Message contents";

			messageArgs = new[] { messageArgument };

			#endregion
		}

		void Print(SampleMessage message)
		{
			logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
				$"Received message: \"{message.Message}\"");
		}

		void Update()
		{
			DeliverMessagesInMailbox();

			SendMessage();

			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.02f;

			if (doSomething)
			{
				if (subscription.Active)
					Unsubscribe();
				else
					Subscribe();
			}
		}

		void DeliverMessagesInMailbox()
		{
			logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
				"Delivering messages from the mailbox");

			messageBusAsSender.DeliverMessagesInMailbox();
		}

		void SendMessage()
		{
			bool genericMessage = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			bool mailboxMessage = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (genericMessage)
			{
				messageArgs[0] = messageText1;

				if (mailboxMessage)
				{
					messageBusAsSender
						.PopMessage<SampleMessage>(out var messageAllGenerics)
						.Write<SampleMessage>(messageAllGenerics, messageArgs)
						.Send<SampleMessage>(messageAllGenerics);

					logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
						"Storing a message in the mailbox");
				}
				else
				{
					logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
						"Delivering generic message immediately");

					messageBusAsSender
						.PopMessage<SampleMessage>(out var messageAllGenerics)
						.Write<SampleMessage>(messageAllGenerics, messageArgs)
						.SendImmediately<SampleMessage>(messageAllGenerics);
				}

				return;
			}

			messageArgs[0] = messageText2;

			if (mailboxMessage)
			{
				messageBusAsSender
					.PopMessage(typeof(SampleMessage), out var messageTypeofs)
					.Write(messageTypeofs, messageArgs)
					.Send(messageTypeofs);
			}
			else
			{
				messageBusAsSender
					.PopMessage(typeof(SampleMessage), out var messageTypeofs)
					.Write(messageTypeofs, messageArgs)
					.SendImmediately(messageTypeofs);
			}
		}

		void Subscribe()
		{
			bool subscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (subscribeWithGeneric)
				messageBusAsReceiver.SubscribeTo<SampleMessage>(subscription);
			else
				messageBusAsReceiver.SubscribeTo(typeof(SampleMessage), subscription);

			logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
				"Subscribed");
		}

		void Unsubscribe()
		{
			bool unsubscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (unsubscribeWithGeneric)
				messageBusAsReceiver.UnsubscribeFrom<SampleMessage>(subscription);
			else
				messageBusAsReceiver.UnsubscribeFrom(typeof(SampleMessage), subscription);

			logger?.Log<NonAllocatingMessageBusSampleBehaviour>(
				"Unsubscribed");
		}

		private class SampleMessage : IMessage
		{
			private string message;

			public string Message
			{
				get => message;
			}

			public void Write(object[] args)
			{
				message = (string)args[0];
			}
		}
	}
}