#if ZLIB_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

using Ionic.Zlib;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsZLib
	{
		public static ISerializerBuilder WithZLibCompression(
			this ISerializerBuilder builder)
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
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}

#endif
