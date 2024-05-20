using System;

using UnityEngine;

using HereticalSolutions.Messaging;
using HereticalSolutions.Messaging.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Samples.MessageBusSample
{
	public class MessageBusSampleBehaviour : MonoBehaviour
	{
		private MessageBus messageBus;

		private IMessageSender messageBusAsSender;

		private IMessageReceiver messageBusAsReceiver;


		private readonly string messageText1 = "Generic message";

		private readonly string messageText2 = "Mailbox message";


		private object[] messageArgs;

		private bool subscriptionActive;


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
				.ToggleLogSource(typeof(MessageBusSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<MessageBusSampleBehaviour>();

			#endregion

			#region Message bus

			var builder = new MessageBusBuilder(loggerResolver);

			builder.AddMessageType<SampleMessage>();

			messageBus = builder.Build();

			messageBusAsSender = (IMessageSender)messageBus;

			messageBusAsReceiver = (IMessageReceiver)messageBus;

			#endregion

			#region Message

			string messageArgument = "Message contents";

			messageArgs = new[] { messageArgument };

			#endregion
		}

		void Print(SampleMessage message)
		{
			logger?.Log<MessageBusSampleBehaviour>(
				$"Received message: \"{message.Message}\"");
		}

		// Update is called once per frame
		void Update()
		{
			DeliverMessagesInMailbox();

			SendMessage();

			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.02f;

			if (doSomething)
			{
				if (subscriptionActive)
					Unsubscribe();
				else
					Subscribe();
			}
		}

		void DeliverMessagesInMailbox()
		{
			logger?.Log<MessageBusSampleBehaviour>(
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

					logger?.Log<MessageBusSampleBehaviour>(
						"Storing a message in the mailbox");
				}
				else
				{
					logger?.Log<MessageBusSampleBehaviour>(
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
				messageBusAsReceiver.SubscribeTo<SampleMessage>(Print);
			else
			{
				Action<SampleMessage> subscription = Print;

				messageBusAsReceiver.SubscribeTo(typeof(SampleMessage), subscription);
			}

			subscriptionActive = true;

			logger?.Log<MessageBusSampleBehaviour>(
				"Subscribed");
		}

		void Unsubscribe()
		{
			bool unsubscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (unsubscribeWithGeneric)
				messageBusAsReceiver.UnsubscribeFrom<SampleMessage>(Print);
			else
			{
				Action<SampleMessage> subscription = Print;

				messageBusAsReceiver.UnsubscribeFrom(typeof(SampleMessage), subscription);
			}

			subscriptionActive = false;

			logger?.Log<MessageBusSampleBehaviour>(
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