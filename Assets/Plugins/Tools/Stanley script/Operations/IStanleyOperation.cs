using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyOperation
	{
		string Opcode { get; }

		string[] Aliases { get; }

		bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment);

		Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}