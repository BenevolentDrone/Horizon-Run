#if JSON_SUPPORT

using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsJSON
	{
		public static ISerializerBuilder ToJSON(
			this ISerializerBuilder builder)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.FormatSerializer =
				JSONPersistenceFactory.BuildJSONSerializer(
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}

#endif