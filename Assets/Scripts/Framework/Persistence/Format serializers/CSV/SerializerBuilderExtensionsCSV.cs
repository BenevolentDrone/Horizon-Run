#if CSV_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsCSV
	{
		public static ISerializerBuilder ToCSV(
			this ISerializerBuilder builder,
			bool includeHeader = true)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			if (builderCasted.DeferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					builderCasted.Logger.TryFormatException(
						builderCasted.GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			builderCasted.DeferredBuildFormatSerializerDelegate = () =>
			{
				if (builderCasted.SerializerContext.FormatSerializer != null)
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				builderCasted.SerializerContext.FormatSerializer =
					CSVPersistenceFactory.BuildCSVSerializer(
						includeHeader,
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}

#endif