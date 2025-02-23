using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PeekFromBottom
		: AStanleyInstructionHandler
	{
		public PeekFromBottom(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_STACK_PEEK_FROM_BOTTOM;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
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
				out var offsetVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				offsetVariable,
				currentLogger))
				return false;

			int offset = offsetVariable.GetValue<int>();

			if (!stack.PeekFromBottom(
				offset,
				out var peekedVariable))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT PEEK VARIABLE FROM BOTTOM. OFFSET:: {offset}");

				return false;
			}

			stack.Push(
				peekedVariable);

			return true;
		}

		#endregion
	}
}