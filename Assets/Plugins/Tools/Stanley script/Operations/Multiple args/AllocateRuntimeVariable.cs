using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class AllocateRuntimeVariable
		: AStanleyOperation
	{
		#region IStanleyOperation

		public override string Opcode => "OP_ALLOC_RTM";

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

			//REMEMBER: when popping from the stack, the order is reversed

			//Get variable name
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

			//Get variable to clone
			if (!stack.Pop(
				out var variableToClone))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable(variableToClone, reportable))
				return false;

			//Add new runtime variable
			if (!environment.AddRuntimeVariable(
				new StanleyCachedVariable(
					variableNameString,
					variableToClone.VariableType,
					variableToClone.GetValue())))
			{
				reportable.Log($"COULD NOT ADD RUNTIME VARIABLE: {variableNameString}");

				return false;
			}

			return true;
		}

		#endregion
	}
}