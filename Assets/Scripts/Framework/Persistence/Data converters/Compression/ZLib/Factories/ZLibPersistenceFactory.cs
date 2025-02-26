#if ZLIB_SUPPORT

using HereticalSolutions.Logging;

using Ionic.Zlib;

namespace HereticalSolutions.Persistence.Factories
{
	public static class ZLibPersistenceFactory
	{
		public static ZLibCompressionConverter BuildZLibCompressionConverter(
			IDataConverter innerDataConverter,
			ILoggerResolver loggerResolver)
		{
			var byteArrayConverter = PersistenceFactory.BuildByteArrayConverter(
				null,
				null,
				loggerResolver);

			return new ZLibCompressionConverter(
				innerDataConverter,
				byteArrayConverter,
				loggerResolver?.GetLogger<ZLibCompressionConverter>());
		}
	}
}

#endif
