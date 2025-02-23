using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class JumpConditional
		: AStanleyInstructionHandler
	{
		public JumpConditional(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_JUMP_CONDITIONAL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(int),
			typeof(bool)
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
				out var pcVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				pcVariable,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var conditionVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<bool>(
				conditionVariable,
				currentLogger))
				return false;

			var pc = pcVariable.GetValue<int>();

			var condition = conditionVariable.GetValue<bool>();

			if (!condition)
			{
				return true;
			}

			stack.SetCurrentProgramCounter(
				pc);

			return true;
		}

		#endregion
	}
}