using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ListConcat
		: AStanleyInstructionHandler
	{
		public ListConcat(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_LIST_CONCAT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyList),
			typeof(StanleyList)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(StanleyList)
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
				out var list1Variable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyList>(
				list1Variable,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var list2Variable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyList>(
				list2Variable,
				currentLogger))
				return false;

			var list1 = list1Variable.GetValue<StanleyList>();

			var list2 = list1Variable.GetValue<StanleyList>();

			var result = StanleyList.Concat(
				list1,
				list2);

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyList),
					result,
					loggerResolver));

			return true;
		}

		#endregion
	}
}