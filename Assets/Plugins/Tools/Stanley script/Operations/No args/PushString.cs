using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class PushString
		: AStanleyOperation
	{
		#region  IStanleyOperation

		public override string Opcode => "OP_PUSH_STR";

		public override bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment)
		{
			if (!AssertOpcode(instructionTokens))
				return false;

			if (!AssertMinInstructionLength(instructionTokens, 2))
				return false;

			if (!AssertInstructionNotEmpty(instructionTokens, 1))
				return false;

			return true;
		}

		public override async Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,
			CancellationToken token)
		{
			var stack = environment as IStackMachine;

			stack.Push(
				new StanleyCachedVariable(
					StanleyConsts.TEMPORARY_VARIABLE,
					typeof(string),
					instructionTokens[1]));

			return true;
		}

		#endregion
	}
}