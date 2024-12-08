using System.Collections.Generic;

namespace HereticalSolutions.Logging
{
	public interface ICompositeLoggerWrapper
		: ILogger
	{
		List<ILogger> InnerLoggers { get; }
	}
}