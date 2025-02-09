using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilderInternal
	{
		ISerializerContext SerializerContext { get; set; }

		Action DeferredBuildFormatSerializerDelegate { get; set; }

		Action DeferredBuildDataConverterDelegate { get; set; }

		Action DeferredBuildSerializationStrategyDelegate { get; set; }

		ILoggerResolver LoggerResolver { get; }

		ILogger Logger { get; }

		void EnsureArgumentsExist();
	}
}