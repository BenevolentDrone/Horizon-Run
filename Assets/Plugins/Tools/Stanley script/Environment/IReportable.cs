namespace HereticalSolutions.StanleyScript
{
	public interface IReportable
	{
		void Log(
			string message);

		string[] GetReport();
	}
}