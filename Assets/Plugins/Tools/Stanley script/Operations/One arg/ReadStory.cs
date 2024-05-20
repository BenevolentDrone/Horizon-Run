using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class ReadStory
		: AStanleyOperation
	{
		#region  IStanleyOperation

		public override string Opcode => "OP_STORY";

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
				out var storyName))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable<string>(storyName, reportable))
				return false;

			var storyNameString = storyName.GetValue<string>();

			if (!AssertValueNotEmpty(storyNameString, reportable))
				return false;

			reportable.Log($"STARTING STORY: {storyNameString}");

			return true;
		}

		#endregion
	}
}