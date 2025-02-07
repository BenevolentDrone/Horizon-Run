#if YAML_SUPPORT

using YamlDotNet.Serialization;

using YamlSerializerBuilder = YamlDotNet.Serialization.SerializerBuilder;
using YamlDeserializerBuilder = YamlDotNet.Serialization.DeserializerBuilder;

using YamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;
using YamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class YAMLPersistenceFactory
	{
		public static YAMLSerializer BuildYAMLSerializer(
			ILoggerResolver loggerResolver)
		{
			return new YAMLSerializer(
				new YamlSerializerBuilder().Build(),
				new YamlDeserializerBuilder().Build(),
				loggerResolver?.GetLogger<YAMLSerializer>());
		}
	}
}

#endif