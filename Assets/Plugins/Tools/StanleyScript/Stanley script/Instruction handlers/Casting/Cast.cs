using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class Cast
		: AStanleyInstructionHandler
	{
		public Cast(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_CAST;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			null,
			typeof(Type)
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
				out var variable1))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!stack.Pop(
				out var variable2))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableTypeAndValue<Type>(
				variable2,
				typeof(string),
				currentLogger))
				return false;

			var originalValue = variable1.GetValue();

			var targetType = variable2.GetValue<Type>();

			if (!stack.TryCast(
				originalValue,
				targetType,
				out var convertedValue))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT CAST {originalValue.GetType()} TO {targetType}");
			}

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					targetType,
					convertedValue,
					loggerResolver));

			return true;
		}

		#endregion
	}
}