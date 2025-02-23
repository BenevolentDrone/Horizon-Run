using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ListInsertAt
		: AStanleyInstructionHandler
	{
		public ListInsertAt(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_LIST_INSERTAT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 3;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyList),
			typeof(int),
			null
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 0;

		public override Type[] ReturnValueTypes => Array.Empty<Type>();

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

			if (!stack.Pop(
				out var valueVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			var list = listVariable.GetValue<StanleyList>();

			var index = indexVariable.GetValue<int>();

			list.InsertAt(
				index,
				valueVariable);

			return true;
		}

		#endregion
	}
}