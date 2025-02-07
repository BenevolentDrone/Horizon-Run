#if YAML_SUPPORT

using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsYAML
	{
		public static ISerializerBuilder ToYAML(
			this ISerializerBuilder builder)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.FormatSerializer =
				YAMLPersistenceFactory.BuildYAMLSerializer(
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}

#endif