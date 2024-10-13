namespace HereticalSolutions.Logging.Factories
{
	public static class LoggersFactoryUnity
	{
		public static UnityDebugLogSink BuildUnityDebugLogSink()
		{
			return new UnityDebugLogSink();
		}
	}
}