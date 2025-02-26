#if ZLIB_SUPPORT

using HereticalSolutions.Logging;

using ZLibNet;

namespace HereticalSolutions.Persistence.Factories
{
	public static class ZLibPersistenceFactory
	{
		public static ZLibCompressionConverter BuildZLibCompressionConverter(
			IDataConverter innerDataConverter,
			CompressionLevel compressionLevel,
			ILoggerResolver loggerResolver)
		{
			var byteArrayConverter = PersistenceFactory.BuildByteArrayConverter(
				null,
				null,
				loggerResolver);

			return new ZLibCompressionConverter(
				innerDataConverter,
				byteArrayConverter,
				compressionLevel,
				loggerResolver?.GetLogger<ZLibCompressionConverter>());
		}
	}
}

#endif
