using System;
using System.Collections.Generic;

namespace HereticalSolutions.Logging
{
	public interface ILoggerBuilder
	{
		ILogger CurrentLogger { get; }

		bool CurrentAllowedByDefault { get; }

		ILoggerBuilder ToggleAllowedByDefault(
			bool allowed);

		ILoggerBuilder ToggleLogSource<TLogSource>(
			bool allowed);

		ILoggerBuilder ToggleLogSource(
			Type logSourceType,
			bool allowed);

		ILoggerBuilder AddSink(ILoggerSink logger);

		ILoggerBuilder Wrap(ILoggerWrapper logger);

		ILoggerBuilder Branch(IEnumerable<ILogger> siblingLoggers);
	}
}