namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyContext
		: IContainsProgramLibrary,
		  IContainsControls,
		  IREPLCompatible
	{
		EExecutionStatus ExecutionStatus { get; }

		IStackMachine StackMachine { get; }

		IStanleyCompiler Compiler { get; }

		IReportMaker ReportMaker { get; }
	}
}