#if JSON_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class JSONPersistenceFactory
	{
		public static JSONSerializer BuildJSONSerializer(
			ILoggerResolver loggerResolver)
		{
			return new JSONSerializer(
				loggerResolver?.GetLogger<JSONSerializer>());
		}
	}
}

#endif