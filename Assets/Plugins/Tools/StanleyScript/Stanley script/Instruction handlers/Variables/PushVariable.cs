using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushVariable
		: AStanleyInstructionHandler
	{
		public PushVariable(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_VARIABLE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string)
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

			if (!stack.CurrentScope.Variables.TryGetVariable(
				variableName,
				out var variable))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT FIND VARIABLE: {variableName}");

				return false;
			}

			stack.Push(
				variable);

			return true;
		}

		#endregion
	}
}