using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ListValueAtIndex
		: AStanleyInstructionHandler
	{
		public ListValueAtIndex(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_LIST_VALATINDEX;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyList),
			typeof(int)
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

			if (!stack.Pop(
				out var indexVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				indexVariable,
				currentLogger))
				return false;

			var list = listVariable.GetValue<StanleyList>();

			var index = indexVariable.GetValue<int>();

			var result = list.GetValueAtIndex(
				index);

			if (result == null)
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT GET VALUE BY INDEX: {index}");

				return false;
			}

			stack.Push(
				result);

			return true;
		}

		#endregion
	}
}