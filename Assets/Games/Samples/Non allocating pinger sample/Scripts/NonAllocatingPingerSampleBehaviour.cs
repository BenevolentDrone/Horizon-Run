using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.NonAllocatingPingerSample
{
	public class NonAllocatingPingerSampleBehaviour : MonoBehaviour
	{
		private IPublisherNoArgs pingerAsPublisher;

		private INonAllocSubscribableNoArgs pingerAsSubscribable;

		private ISubscription subscription;


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
				.ToggleLogSource(typeof(NonAllocatingPingerSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<NonAllocatingPingerSampleBehaviour>();

			#endregion

			#region Pinger

			var pinger = DelegatesFactory.BuildNonAllocPinger(loggerResolver);

			pingerAsPublisher = (IPublisherNoArgs)pinger;

			pingerAsSubscribable = (INonAllocSubscribableNoArgs)pinger;

			#endregion

			#region Subscription

			subscription = DelegatesFactory.BuildSubscriptionNoArgs(Print, loggerResolver);

			#endregion
		}

		void Print()
		{
			logger?.Log<NonAllocatingPingerSampleBehaviour>(
				"Ping");
		}

		void Update()
		{
			Ping();

			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

			if (doSomething)
			{
				if (subscription.Active)
					Unsubscribe();
				else
					Subscribe();
			}
		}

		void Ping()
		{
			pingerAsPublisher.Publish();
		}

		void Subscribe()
		{
			bool subscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (subscribeWithGeneric)
				pingerAsSubscribable.Subscribe(subscription);
			else
				pingerAsSubscribable.Subscribe(subscription);

			logger?.Log<NonAllocatingPingerSampleBehaviour>(
				"Subscribed");
		}

		void Unsubscribe()
		{
			bool unsubscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (unsubscribeWithGeneric)
				pingerAsSubscribable.Unsubscribe(subscription);
			else
				pingerAsSubscribable.Unsubscribe(subscription);

			logger?.Log<NonAllocatingPingerSampleBehaviour>(
				"Unsubscribed");
		}
	}
}