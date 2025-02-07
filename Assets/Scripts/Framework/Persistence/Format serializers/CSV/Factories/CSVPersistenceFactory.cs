#if CSV_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class CSVPersistenceFactory
	{
		public static CSVSerializer BuildCSVSerializer(
			bool includeHeader,
			ILoggerResolver loggerResolver)
		{
			return new CSVSerializer(
				includeHeader,
				loggerResolver?.GetLogger<CSVSerializer>());
		}
	}
}

#endif