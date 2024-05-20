using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.PingerSample
{
	public class PingerSampleBehaviour : MonoBehaviour
	{
		private IPublisherNoArgs pingerAsPublisher;

		private ISubscribableNoArgs pingerAsSubscribable;


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
				.ToggleLogSource(typeof(PingerSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<PingerSampleBehaviour>();

			#endregion

			#region Pinger

			var pinger = DelegatesFactory.BuildPinger();

			pingerAsPublisher = (IPublisherNoArgs)pinger;

			pingerAsSubscribable = (ISubscribableNoArgs)pinger;

			#endregion
		}

		void Print()
		{
			logger?.Log<PingerSampleBehaviour>("Ping");
		}

		void Update()
		{
			Ping();

			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.02f;

			if (doSomething)
			{
				if (subscriptionActive)
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
				pingerAsSubscribable.Subscribe(Print);
			else
				pingerAsSubscribable.Subscribe(Print);

			subscriptionActive = true;

			logger?.Log<PingerSampleBehaviour>(
				"Subscribed");
		}

		void Unsubscribe()
		{
			bool unsubscribeWithGeneric = UnityEngine.Random.Range(0f, 1f) > 0.5f;

			if (unsubscribeWithGeneric)
				pingerAsSubscribable.Unsubscribe(Print);
			else
				pingerAsSubscribable.Unsubscribe(Print);

			subscriptionActive = false;

			logger?.Log<PingerSampleBehaviour>(
				"Unsubscribed");
		}
	}
}