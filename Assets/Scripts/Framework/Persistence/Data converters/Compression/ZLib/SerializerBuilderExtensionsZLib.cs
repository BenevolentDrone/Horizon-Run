#if ZLIB_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

using ZLibNet;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsZLib
	{
		public static ISerializerBuilder WithZLibCompression(
			this ISerializerBuilder builder,
			CompressionLevel compressionLevel)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.DeferredBuildDataConverterDelegate += () =>
			{
				if (builderCasted.SerializerContext.DataConverter == null)
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				builderCasted.SerializerContext.DataConverter =
					ZLibPersistenceFactory.BuildZLibCompressionConverter(
						builderCasted.SerializerContext.DataConverter,
						compressionLevel,
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}

#endif
