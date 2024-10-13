using System.Collections.Generic;

namespace HereticalSolutions.Logging
{
	public interface ICompositeLoggerWrapper
		: ILogger
	{
		IEnumerable<ILogger> InnerLoggers { get; }
	}
}