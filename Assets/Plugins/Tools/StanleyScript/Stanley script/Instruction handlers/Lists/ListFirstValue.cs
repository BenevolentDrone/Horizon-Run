using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ListFirstValue
		: AStanleyInstructionHandler
	{
		public ListFirstValue(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_LIST_FIRSTVAL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyList)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			null
		};

		#endregion

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var currentLogger = SelectLogger(
				context);

			var stack = context.StackMachine;

			if (!stack.Pop(
				out var listVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyList>(
				listVariable,
				currentLogger))
				return false;

			var list = listVariable.GetValue<StanleyList>();

			var result = list.FirstValue;

			if (result == null)
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT GET FIRST VALUE IN LIST");

				return false;
			}

			stack.Push(
				result);

			return true;
		}

		#endregion
	}
}