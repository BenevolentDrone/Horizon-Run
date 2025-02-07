#if PROTOBUF_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class ProtobufPersistenceFactory
	{
		public static ProtobufSerializer BuildProtobufSerializer(
			ILoggerResolver loggerResolver)
		{
			return new ProtobufSerializer(
				loggerResolver?.GetLogger<ProtobufSerializer>());
		}
	}
}

#endif