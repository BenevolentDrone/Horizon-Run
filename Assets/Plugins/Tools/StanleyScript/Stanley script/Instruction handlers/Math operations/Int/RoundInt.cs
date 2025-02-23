using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class RoundInt
		: AStanleyInstructionHandler
	{
		public RoundInt(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_ROUND;

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
			typeof(int)
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

			if (!AssertVariableType<int>(
				variable1,
				currentLogger))
				return false;

			var value1 = variable1.GetValue<int>();

			var result = Convert.ToInt32(
				Math.Round(
					(double)value1));

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(int),
					result,
					loggerResolver));

			return true;
		}

		#endregion
	}
}