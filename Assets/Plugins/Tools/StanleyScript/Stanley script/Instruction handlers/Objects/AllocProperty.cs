using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class AllocProperty
		: AStanleyInstructionHandler
	{
		public AllocProperty(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_ALLOC_PROPERTY;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 3;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyObject),
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

			if (!stack.Pop(
				out var propertyTypeVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<Type>(
				propertyTypeVariable,
				currentLogger))
				return false;

			var propertyName = propertyNameVariable.GetValue<string>();
			
			var propertyType = propertyTypeVariable.GetValue<Type>();

			// Try to create Stanley objects first
			object value;
			
			if (propertyType == typeof(StanleyProgramObject))
			{
				value = StanleyFactory.BuildStanleyProgramObject(
					context,
					loggerResolver,
					
					asyncContext);
			}
			else if (propertyType == typeof(StanleyObject))
			{
				value = StanleyFactory.BuildStanleyObject(); // No args needed
			}
			else if (propertyType == typeof(StanleyList))
			{
				value = StanleyFactory.BuildStanleyList(); // No args needed
			}
			else if (propertyType == typeof(StanleyEvent))
			{
				// Event needs a poller, jumpToLabel, and label
				value = StanleyFactory.BuildStanleyEvent(
					null,			// Default, no poller
					false,	// Don't jump to label by default
					null);			// No label by default
			}
			else if (propertyType == typeof(StanleyDelegate))
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
				value = StanleyFactory.GetDefaultValue(propertyType);
			}

			var propertyVariable = StanleyFactory.BuildValueVariable(
				propertyName,
				propertyType,
				value,
				loggerResolver);

			var @object = objectVariable.GetValue<StanleyObject>();

			bool result = @object.Properties.TryAddVariable(propertyVariable);

			if (result)
			{
				IVariableEventProcessor eventProcessor = stack.CurrentScope.Variables
					as IVariableEventProcessor;

				eventProcessor?.ProcessVariableAddedEvent(
					propertyVariable);
			}

			return true;
		}
		
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

			if (!stack.Pop(
				out var propertyTypeVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<Type>(
				propertyTypeVariable,
				currentLogger))
				return false;

			var propertyName = propertyNameVariable.GetValue<string>();
			
			var propertyType = propertyTypeVariable.GetValue<Type>();

			// Try to create Stanley objects first
			object value;
			
			if (propertyType == typeof(StanleyProgramObject))
			{
				value = StanleyFactory.BuildStanleyProgramObject(
					context,
					loggerResolver);
			}
			else if (propertyType == typeof(StanleyObject))
			{
				value = StanleyFactory.BuildStanleyObject(); // No args needed
			}
			else if (propertyType == typeof(StanleyList))
			{
				value = StanleyFactory.BuildStanleyList(); // No args needed
			}
			else if (propertyType == typeof(StanleyEvent))
			{
				// Event needs a poller, jumpToLabel, and label
				value = StanleyFactory.BuildStanleyEvent(
					null,			// Default, no poller
					false,	// Don't jump to label by default
					null);			// No label by default
			}
			else if (propertyType == typeof(StanleyDelegate))
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
				value = StanleyFactory.GetDefaultValue(propertyType);
			}

			var propertyVariable = StanleyFactory.BuildValueVariable(
				propertyName,
				propertyType,
				value,
				loggerResolver);

			var @object = objectVariable.GetValue<StanleyObject>();

			bool result = @object.Properties.TryAddVariable(propertyVariable);

			if (result)
			{
				IVariableEventProcessor eventProcessor = stack.CurrentScope.Variables
					as IVariableEventProcessor;

				eventProcessor?.ProcessVariableAddedEvent(
					propertyVariable);
			}

			return true;
		}

		#endregion
	}
}