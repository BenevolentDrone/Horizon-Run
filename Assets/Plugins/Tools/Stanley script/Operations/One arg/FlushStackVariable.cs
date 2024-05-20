using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class FlushStackVariable
		: AStanleyOperation
	{
		#region  IStanleyOperation

		public override string Opcode => "OP_FLUSH";

		public override bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment)
		{
			if (!AssertOpcode(instructionTokens))
				return false;

			return true;
		}

		public override async Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,
			CancellationToken token)
		{
			var stack = environment as IStackMachine;

			var reportable = environment as IReportable;

			if (!stack.Pop(
				out var _))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			return true;
		}

		#endregion
	}
}