using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class FreeVariable
		: AStanleyInstructionHandler
	{
		public FreeVariable(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_FREE_VARIABLE;

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
				out var variableNameVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				variableNameVariable,
				currentLogger))
				return false;

			var variableName = variableNameVariable.GetValue<string>();

			stack.CurrentScope.Variables.TryRemoveVariable(
				variableName);

			return true;
		}

		#endregion
	}
}