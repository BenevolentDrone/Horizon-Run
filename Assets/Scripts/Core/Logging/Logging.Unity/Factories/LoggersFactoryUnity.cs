namespace HereticalSolutions.Logging.Factories
{
	public static class LoggersFactoryUnity
	{
		public static UnityDebugLogger BuildUnityDebugLogger(
			bool printLogs = true,
			bool printWarnings = true,
			bool printErrors = true)
		{
			return new UnityDebugLogger(
				printLogs,
				printWarnings,
				printErrors);
		}
	}
}