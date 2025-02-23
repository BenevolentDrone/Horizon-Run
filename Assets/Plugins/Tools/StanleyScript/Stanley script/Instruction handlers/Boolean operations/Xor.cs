using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class Xor
		: AStanleyInstructionHandler
	{
		public Xor(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_XOR;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(bool),
			typeof(bool)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(bool)
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
				out var variable1))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<bool>(
				variable1,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var variable2))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<bool>(
				variable2,
				currentLogger))
				return false;

			var value1 = variable1.GetValue<bool>();

			var value2 = variable2.GetValue<bool>();

			var result = value1 ^ value2;

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(bool),
					result,
					loggerResolver));

			return true;
		}

		#endregion
	}
}