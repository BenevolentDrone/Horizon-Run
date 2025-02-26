#if LZ4_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsLZ4
	{
		public static ISerializerBuilder WithLZ4Compression(
			this ISerializerBuilder builder,
			LZ4Level compressionLevel)
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
					LZ4PersistenceFactory.BuildLZ4CompressionConverter(
						builderCasted.SerializerContext.DataConverter,
						compressionLevel,
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}

#endif