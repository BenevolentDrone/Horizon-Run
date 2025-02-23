using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class SetLC
		: AStanleyInstructionHandler
	{
		public SetLC(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_STACK_SET_LC;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(int)
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
				out var lcVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				lcVariable,
				currentLogger))
				return false;

			var lc = lcVariable.GetValue<int>();

			stack.SetCurrentLineCounter(
				lc);

			return true;
		}

		#endregion
	}
}