using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsInstructionHandlers
	{
		IEnumerable<string> AllInstructions { get; }

		IEnumerable<IStanleyInstructionHandler> AllInstructionHandlers { get; }

		bool GetHandlers(
			string instructionOrAlias,
			out IEnumerable<IStanleyInstructionHandler> handlers);

		bool AddHandler(
			IStanleyInstructionHandler handler);

		bool RemoveHandler(
			IStanleyInstructionHandler handler);
	}
}