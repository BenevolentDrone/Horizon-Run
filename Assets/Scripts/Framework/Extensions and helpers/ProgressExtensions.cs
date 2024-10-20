using System;

using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class ProgressExtensions
	{
		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					logger?.Log(
						$"PROGRESS: {String.Format("{0:0.00}", value)}");

					progress.Report(value);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressWithHandler(
			this IProgress<float> progress,
			EventHandler<float> progressChangedHandler)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += progressChangedHandler;

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressWithCalculationDelegate(
			this IProgress<float> progress,
			Func<float, float> totalProgressCalculationDelegate,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					var totalProgress = totalProgressCalculationDelegate.Invoke(
						value);

					logger?.Log(
						$"LOCAL PROGRESS: {String.Format("{0:0.00}", value)} PROGRESS: {String.Format("{0:0.00}", totalProgress)}");

					progress.Report(
						totalProgress);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressWithRange(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float range = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					var totalProgress = range * value + totalProgressStart;

					logger?.Log(
						$"LOCAL PROGRESS: {String.Format("{0:0.00}", value)} START: {String.Format("{0:0.00}", totalProgressStart)} FINISH: {String.Format("{0:0.00}", totalProgressFinish)} SCALE: {String.Format("{0:0.00}", range)} PROGRESS: {String.Format("{0:0.00}", totalProgress)}");

					progress.Report(totalProgress);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressWithRangeAndCalculationDelegate(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			Func<float, float> localProgressCalculationDelegate,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float range = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					var totalProgress = range * localProgressCalculationDelegate.Invoke(value) + totalProgressStart;

					logger?.Log(
						$"LOCAL PROGRESS: {String.Format("{0:0.00}", value)} START: {String.Format("{0:0.00}", totalProgressStart)} FINISH: {String.Format("{0:0.00}", totalProgressFinish)} SCALE: {String.Format("{0:0.00}", range)} PROGRESS: {String.Format("{0:0.00}", totalProgress)}");

					progress.Report(totalProgress);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressForStep(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			int index,
			int count,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float range = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					var totalProgress = range * ((value + (float)index) / count) + totalProgressStart;

					logger?.Log(
						$"LOCAL PROGRESS: {String.Format("{0:0.00}", value)} START: {String.Format("{0:0.00}", totalProgressStart)} FINISH: {String.Format("{0:0.00}", totalProgressFinish)} INDEX: {String.Format("{0:0.00}", index)} COUNT: {String.Format("{0:0.00}", count)} RANGE: {String.Format("{0:0.00}", range)} PROGRESS: {String.Format("{0:0.00}", totalProgress)}");

					progress.Report(totalProgress);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgressForStepWithinList(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			List<float> localProgressValues,
			int index,
			int count,
			ILogger logger = null)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float range = totalProgressFinish - totalProgressStart;


				localProgressValues.Add(0f);


				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					localProgressValues[index] = value;


					float totalProgressForList = 0f;

					foreach (var assetImportProgress in localProgressValues)
					{
						totalProgressForList += assetImportProgress;
					}

					var totalProgress = range * (totalProgressForList / (float)count) + totalProgressStart;

					logger?.Log(
						$"LOCAL PROGRESS: {String.Format("{0:0.00}", value)} START: {String.Format("{0:0.00}", totalProgressStart)} FINISH: {String.Format("{0:0.00}", totalProgressFinish)} INDEX: {String.Format("{0:0.00}", index)} COUNT: {String.Format("{0:0.00}", count)} RANGE: {String.Format("{0:0.00}", range)} LIST PROGRESS: {String.Format("{0:0.00}", totalProgressForList)} PROGRESS: {String.Format("{0:0.00}", totalProgress)}");

					progress.Report(totalProgress);
				};

				localProgress = localProgressInstance;
			}
			
			return localProgress;
		}
	}
}