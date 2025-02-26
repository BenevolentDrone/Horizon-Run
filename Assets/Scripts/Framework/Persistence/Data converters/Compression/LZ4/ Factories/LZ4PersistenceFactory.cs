#if LZ4_SUPPORT

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence.Factories
{
	public static class LZ4PersistenceFactory
	{
		public static LZ4CompressionConverter BuildLZ4CompressionConverter(
			IDataConverter innerDataConverter,
			LZ4Level compressionLevel,
			ILoggerResolver loggerResolver)
		{
			var byteArrayConverter = PersistenceFactory.BuildByteArrayConverter(
				null,
				null,
				loggerResolver);

			return new LZ4CompressionConverter(
				innerDataConverter,
				byteArrayConverter,
				compressionLevel,
				loggerResolver?.GetLogger<LZ4CompressionConverter>());
		}
	}
}

#endif