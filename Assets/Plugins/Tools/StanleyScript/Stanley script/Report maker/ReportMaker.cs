using System;
using System.Globalization;

using HereticalSolutions.Persistence;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ReportMaker
		: IReportMaker
	{
		private ILogger reportLogger;

		private ISerializer serializer;

		#region IReportMaker

		public void InitializeNewReport(
			IStanleyContext context)
		{
			string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

			dateTimeNow = dateTimeNow.Replace('T', '_');

			dateTimeNow = dateTimeNow.Replace(':', '-');

			string reportFileName = $"Report {dateTimeNow}";
			
			StanleyFactory.BuildReportMakerLogger(
				context,
				reportFileName,
				out reportLogger,
				out serializer);

			//Open stream

			var streamStrategy = serializer
				.Context
				.SerializationStrategy
				as IStrategyWithStream;

			streamStrategy?.InitializeAppend();
		}

		public ILogger ReportLogger => reportLogger;

		public void FinalizeReport()
		{
			//Close stream

			if (serializer != null)
			{
				var streamStrategy = serializer
					.Context
					.SerializationStrategy
					as IStrategyWithStream;

				streamStrategy?.FinalizeAppend();
			}

			this.reportLogger = null;

			this.serializer = null;
		}

		#endregion
	}
}