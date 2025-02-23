using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class FreeProperty
		: AStanleyInstructionHandler
	{
		public FreeProperty(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_FREE_PROPERTY;

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

			if (@object.Properties.TryGetVariable(
				propertyName,
				out var freedVariable))
			{
				IVariableEventProcessor eventProcessor = stack.CurrentScope.Variables
					as IVariableEventProcessor;

				eventProcessor?.ProcessVariableAddedEvent(
					freedVariable);
			}

			@object.Properties.TryRemoveVariable(
				propertyName);

			return true;
		}

		#endregion
	}
}