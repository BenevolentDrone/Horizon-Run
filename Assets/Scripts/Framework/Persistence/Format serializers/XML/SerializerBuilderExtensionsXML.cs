#if XML_SUPPORT

using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsXML
	{
		public static ISerializerBuilder ToXML(
			this ISerializerBuilder builder)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.FormatSerializer =
				XMLPersistenceFactory.BuildXMLSerializer(
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}

#endif