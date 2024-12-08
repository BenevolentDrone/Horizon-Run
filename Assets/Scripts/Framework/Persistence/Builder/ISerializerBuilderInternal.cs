using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilderInternal
	{
		ISerializerContext SerializerContext { get; set; }

		ILoggerResolver LoggerResolver { get; }
	}
}