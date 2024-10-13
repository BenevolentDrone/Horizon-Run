using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class LoadingProgressWindow : MonoBehaviour
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject(Id = "Loading progress")]
		private IProgress<float> loadingProgress;

		[Inject(Id = "Loading progress logger")]
		private ICompositeLoggerWrapper loadingProgressLogger;


		[SerializeField]
		private TextMeshProUGUI textComponent;

		[SerializeField]
		private Image progressBar;

		[SerializeField]
		private bool sinkIntoLogger;


		private ILogger localProgressLogger;

		private ILogger logger;

		private void Awake()
		{
			logger = loggerResolver?.GetLogger(GetType());


			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(true)
				.AddSink(
					new TMProUGUISink(
						logTextComponent: textComponent,
						warningTextComponent: textComponent,
						errorTextComponent: textComponent));

			if (sinkIntoLogger)
			{						
				loggerBuilder
					.Branch(
						new []
						{
							logger
						});
			}

			localProgressLogger = loggerBuilder.CurrentLogger;


			IEnumerable<ILogger> innerLoggers = loadingProgressLogger.InnerLoggers;

			if (innerLoggers is ICollection<ILogger> siblingsList)
			{
				siblingsList.Add(localProgressLogger);
			}
			else
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"COULD NOT ADD LOCAL PROGRESS LOGGER TO LOADING PROGRESS LOGGER'S INNER LOGGERS COLLECTION"));
			}

			if (loadingProgress is Progress<float> progressDowncasted)
			{
				progressDowncasted.ProgressChanged += (sender, value) =>
				{
					progressBar.fillAmount = value;
				};
			}
			else
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"COULD NOT DOWNCAST LOADING PROGRESS TO Progress<float>"));
			}
		}
	}
}