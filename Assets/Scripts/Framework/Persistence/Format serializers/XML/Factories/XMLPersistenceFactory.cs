#if XML_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class XMLPersistenceFactory
	{
		public static XMLSerializer BuildXMLSerializer(
			ILoggerResolver loggerResolver)
		{
			return new XMLSerializer(
				loggerResolver?.GetLogger<XMLSerializer>());
		}
	}
}

#endif