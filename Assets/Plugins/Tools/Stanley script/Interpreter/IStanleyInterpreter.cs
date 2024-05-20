using System;

namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyInterpreter
	{
		//IRuntimeEnvironment Environment { get; }

		//IExecutable Executable { get; }

		string[] InterpretToOpcode(string script);

		//public void Execute(string script);
	}
}