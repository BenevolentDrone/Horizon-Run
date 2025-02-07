#if CSV_SUPPORT

using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsCSV
	{
		public static ISerializerBuilder ToCSV(
			this ISerializerBuilder builder,
			bool includeHeader = true)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.FormatSerializer =
				CSVPersistenceFactory.BuildCSVSerializer(
					includeHeader,
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}

#endif