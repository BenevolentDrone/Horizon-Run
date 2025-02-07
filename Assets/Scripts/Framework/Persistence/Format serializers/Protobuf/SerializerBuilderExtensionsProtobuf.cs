#if PROTOBUF_SUPPORT

using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsProtobuf
	{
		public static ISerializerBuilder ToProtobuf(
			this ISerializerBuilder builder)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.FormatSerializer =
				ProtobufPersistenceFactory.BuildProtobufSerializer(
				builderCasted.LoggerResolver);

			return builder;
		}
	}
}

#endif