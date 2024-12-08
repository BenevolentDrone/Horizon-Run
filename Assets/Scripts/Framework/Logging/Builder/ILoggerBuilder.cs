using System;
using System.Collections.Generic;

namespace HereticalSolutions.Logging
{
	public interface ILoggerBuilder
	{
		ILogger RootLogger { get; }

		ILogger CurrentLogger { get; set; }

		bool CurrentAllowedByDefault { get; }


		ILoggerBuilder NewLogger();


		ILoggerBuilder ToggleAllowedByDefault(
			bool allowed);


		ILoggerBuilder ToggleLogSource<TLogSource>(
			bool allowed);

		ILoggerBuilder ToggleLogSource(
			Type logSourceType,
			bool allowed);


		ILoggerBuilder AddSink(
			ILoggerSink logger);

		ILoggerBuilder AddWrapperBelow(
			ILoggerWrapper logger);

		ILoggerBuilder Branch();

		
		ILoggerResolver Build();
	}
}