namespace HereticalSolutions.Logging
{
	public interface ILoggerWrapper
		: ILogger
	{
		ILogger InnerLogger { get; }
	}
}