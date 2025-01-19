using System;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class AsyncExtensions
	{
		public static AsyncExecutionContext CreateLocalProgress(
			this AsyncExecutionContext asyncContext,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgress(
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressWithHandler(
			this AsyncExecutionContext asyncContext,
			EventHandler<float> progressChangedHandler)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressWithHandler(
					progressChangedHandler),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressWithCalculationDelegate(
			this AsyncExecutionContext asyncContext,
			Func<float, float> totalProgressCalculationDelegate,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressWithCalculationDelegate(
					totalProgressCalculationDelegate,
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressWithRange(
			this AsyncExecutionContext asyncContext,
			float totalProgressStart,
			float totalProgressFinish,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressWithRange(
					totalProgressStart,
					totalProgressFinish,
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressWithRangeAndCalculationDelegate(
			this AsyncExecutionContext asyncContext,
			float totalProgressStart,
			float totalProgressFinish,
			Func<float, float> localProgressCalculationDelegate,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressWithRangeAndCalculationDelegate(
					totalProgressStart,
					totalProgressFinish,
					localProgressCalculationDelegate,
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressForStep(
			this AsyncExecutionContext asyncContext,
			float totalProgressStart,
			float totalProgressFinish,
			int index,
			int count,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressForStep(
					totalProgressStart,
					totalProgressFinish,
					index,
					count,
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}

		public static AsyncExecutionContext CreateLocalProgressForStepWithinList(
			this AsyncExecutionContext asyncContext,
			float totalProgressStart,
			float totalProgressFinish,
			List<float> localProgressValues,
			int index,
			int count,
			ILogger logger)
		{
			if (asyncContext == null)
			{
				return null;
			}

			return new AsyncExecutionContext
			{
				CancellationToken = asyncContext.CancellationToken,

				Progress = asyncContext.Progress.CreateLocalProgressForStepWithinList(
					totalProgressStart,
					totalProgressFinish,
					localProgressValues,
					index,
					count,
					logger),

				ProgressLogger = asyncContext.ProgressLogger
			};
		}
	}
}