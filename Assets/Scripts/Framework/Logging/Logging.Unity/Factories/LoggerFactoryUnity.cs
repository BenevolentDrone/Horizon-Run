namespace HereticalSolutions.Logging.Factories
{
	public static class LoggerFactoryUnity
	{
		public static UnityDebugLogSink BuildUnityDebugLogSink()
		{
			return new UnityDebugLogSink();
		}
	}
}