using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class PushRuntimeVariable
		: AStanleyOperation
	{
		#region IStanleyOperation

		public override string Opcode => "OP_PUSH_RTM";

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
				out var variableName))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable<string>(variableName, reportable))
				return false;

			var variableNameString = variableName.GetValue<string>();

			if (!AssertValueNotEmpty(variableNameString, reportable))
				return false;

			if (!environment.GetRuntimeVariable(
				variableNameString,
				out var runtimeVariable))
			{
				reportable.Log($"RUNTIME VARIABLE NOT FOUND: {variableNameString}");

				return false;
			}

			stack.Push(
				runtimeVariable);

			return true;
		}

		#endregion
	}
}