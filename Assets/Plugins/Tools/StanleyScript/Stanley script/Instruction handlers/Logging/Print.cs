using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class Print
		: AStanleyInstructionHandler
	{
		public Print(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PRINT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string)
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
				out var printStringVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				printStringVariable,
				currentLogger))
				return false;

			var printString = printStringVariable.GetValue<string>();

			currentLogger?.Log(
				GetType(),
				printString);

			return true;
		}

		#endregion
	}
}