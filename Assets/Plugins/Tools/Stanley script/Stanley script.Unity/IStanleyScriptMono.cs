namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyScriptMono
	{
		IStanleyInterpreter Interpreter { get; }

		IRuntimeEnvironment Environment { get; }
	}
}