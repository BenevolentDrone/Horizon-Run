using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public interface IReportMaker
	{
		void InitializeNewReport(
			IStanleyContext context);

		ILogger ReportLogger { get; }

		void FinalizeReport();
	}
}