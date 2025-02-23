using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsInterpreter
	{
		string[] InstructionsListing { get; }

		int LineCounter { get; }

		void LoadInstructions(
			string[] instructions);

		bool ExecuteNext(
			IStanleyContextInternal context);

		Task<bool> ExecuteNextAsync(
			IStanleyContextInternal context,

			//Async tail
			AsyncExecutionContext asyncContext);

		bool ExecuteImmediately(
			IStanleyContextInternal context,
			string instructionLine);

		Task<bool> ExecuteImmediatelyAsync(
			IStanleyContextInternal context,
			string instructionLine,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}