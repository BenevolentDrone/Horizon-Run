using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushProperty
		: AStanleyInstructionHandler
	{
		public PushProperty(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_PROPERTY;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyObject),
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
				out var objectVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyObject>(
				objectVariable,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var propertyNameVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				propertyNameVariable,
				currentLogger))
				return false;

			var @object = objectVariable.GetValue<StanleyObject>();

			var propertyName = propertyNameVariable.GetValue<string>();

			if (!@object.Properties.TryGetVariable(
				propertyName,
				out var property))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT GET PROPERTY: {propertyName}");

				return false;
			}

			stack.Push(
				property);

			return true;
		}

		#endregion
	}
}