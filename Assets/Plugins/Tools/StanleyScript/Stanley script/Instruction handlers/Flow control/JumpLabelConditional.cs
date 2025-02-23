using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class JumpLabelConditional
		: AStanleyInstructionHandler
	{
		public JumpLabelConditional(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string),
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
				out var labelStringVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				labelStringVariable,
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

			var labelString = labelStringVariable.GetValue<string>();

			var condition = conditionVariable.GetValue<bool>();

			if (!condition)
			{
				return true;
			}

			if (StanleyInlineDelegates.TryGetJumpTableVariable(
				stack.CurrentScope,
				out var jumpTableVariable))
			{
				var jumpTable = jumpTableVariable.GetValue<StanleyObject>();

				if (!jumpTable.Properties.TryGetVariable(
					labelString,
					out var pcVariable))
				{
					currentLogger?.LogError(
						GetType(),
						$"COULD NOT FIND LABEL: {labelString}");

					return false;
				}

				int pc = pcVariable.GetValue<int>();

				stack.SetCurrentProgramCounter(
					pc);

				return true;
			}

			currentLogger?.LogError(
				GetType(),
				$"COULD NOT FIND JUMP TABLE");

			return false;
		}

		#endregion
	}
}