using System;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public class AllocVariable
		: AStanleyInstructionHandler
	{
		public AllocVariable(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_ALLOC_VARIABLE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string),
			typeof(Type)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 0;

		public override Type[] ReturnValueTypes => Array.Empty<Type>();

		#endregion

		public override async Task<bool> Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var stack = context.StackMachine;

			var currentLogger = SelectLogger(
				context);

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

			if (!stack.Pop(
				out var variableTypeVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<Type>(
				variableTypeVariable,
				currentLogger))
				return false;

			var variableName = variableNameVariable.GetValue<string>();
			var variableType = variableTypeVariable.GetValue<Type>();

			// Try to create Stanley objects first
			object value;
			
			if (variableType == typeof(StanleyProgramObject))
			{
				value = StanleyFactory.BuildStanleyProgramObject(
					context,
					loggerResolver,
					
					asyncContext);
			}
			else if (variableType == typeof(StanleyObject))
			{
				value = StanleyFactory.BuildStanleyObject(); // No args needed
			}
			else if (variableType == typeof(StanleyList))
			{
				value = StanleyFactory.BuildStanleyList(); // No args needed
			}
			else if (variableType == typeof(StanleyEvent))
			{
				// Event needs a poller, jumpToLabel, and label
				value = StanleyFactory.BuildStanleyEvent(
					null,			// Default, no poller
					false,	// Don't jump to label by default
					null);			// No label by default
			}
			else if (variableType == typeof(StanleyDelegate))
			{
				// Delegate needs methodInfo and awaitCompletion
				value = StanleyFactory.BuildStanleyDelegate(
					null,			// No target by default
					null,		// No method info by default
					false);	// Don't await completion by default
			}
			else
			{
				// Fallback to default value for primitive types
				value = StanleyFactory.GetDefaultValue(variableType);
			}

			stack.CurrentScope.Variables.TryAddVariable(
				StanleyFactory.BuildValueVariable(
					variableName,
					variableType,
					value,
					loggerResolver));

			return true;
		}
		
		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var stack = context.StackMachine;

			var currentLogger = SelectLogger(
				context);

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

			if (!stack.Pop(
				out var variableTypeVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<Type>(
				variableTypeVariable,
				currentLogger))
				return false;

			var variableName = variableNameVariable.GetValue<string>();
			var variableType = variableTypeVariable.GetValue<Type>();

			// Try to create Stanley objects first
			object value;
			
			if (variableType == typeof(StanleyProgramObject))
			{
				value = StanleyFactory.BuildStanleyProgramObject(
					context,
					loggerResolver); // No args needed
			}
			else if (variableType == typeof(StanleyObject))
			{
				value = StanleyFactory.BuildStanleyObject(); // No args needed
			}
			else if (variableType == typeof(StanleyList))
			{
				value = StanleyFactory.BuildStanleyList(); // No args needed
			}
			else if (variableType == typeof(StanleyEvent))
			{
				// Event needs a poller, jumpToLabel, and label
				value = StanleyFactory.BuildStanleyEvent(
					null,			// Default, no poller
					false,	// Don't jump to label by default
					null);			// No label by default
			}
			else if (variableType == typeof(StanleyDelegate))
			{
				// Delegate needs methodInfo and awaitCompletion
				value = StanleyFactory.BuildStanleyDelegate(
					null,			// No target by default
					null,		// No method info by default
					false);	// Don't await completion by default
			}
			else
			{
				// Fallback to default value for primitive types
				value = StanleyFactory.GetDefaultValue(variableType);
			}

			stack.CurrentScope.Variables.TryAddVariable(
				StanleyFactory.BuildValueVariable(
					variableName,
					variableType,
					value,
					loggerResolver));

			return true;
		}

		#endregion
	}
}