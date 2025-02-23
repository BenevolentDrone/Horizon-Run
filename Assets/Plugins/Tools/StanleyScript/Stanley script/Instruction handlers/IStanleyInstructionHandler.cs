using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyInstructionHandler
	{
		#region Instruction

		string Instruction { get; }

		string[] Aliases { get; }

		#endregion

		#region Arguments

		int ArgumentsCount { get; }

		Type[] ArgumentTypes { get; }

		#endregion

		#region Return values

		int ReturnValuesCount { get; }

		Type[] ReturnValueTypes { get; }

		#endregion

		bool WillHandle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens);

		bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens);

		Task<bool> Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}