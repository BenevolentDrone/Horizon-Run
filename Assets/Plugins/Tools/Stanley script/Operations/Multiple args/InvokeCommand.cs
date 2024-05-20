using System;

using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class InvokeCommand
		: AStanleyOperation
	{
		#region IStanleyOperation

		public override string Opcode => "OP_INVOKE";

		public string[] Aliases => null;

		public override bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment)
		{
			if (!AssertOpcode(instructionTokens))
				return false;

			//if (!AssertMinInstructionLength(instructionTokens, 2))
			//	return false;

			//if (!AssertInstructionNotEmpty(instructionTokens, 1))
			//	return false;

			return true;
		}

		public override async Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,
			CancellationToken token)
		{
			var stack = environment as IStackMachine;

			var reportable = environment as IReportable;

			var REPL = environment as IREPL;

			//REMEMBER: when popping from the stack, the order is reversed

			//string opcode = instructionTokens[1];

			//int argumentsAmount = Convert.ToInt32(instructionTokens[2]);

			//string instruction = $"{opcode} {argumentsAmount}";

			if (!stack.Pop(
				out var opcode))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable<string>(opcode, reportable))
				return false;

			var opcodeString = opcode.GetValue<string>();

			if (!AssertValueNotEmpty(opcodeString, reportable))
				return false;

			bool result = await REPL.Execute(
				opcodeString,
				token);
				//.ThrowExceptions();

			return result;
		}

		#endregion
	}
}