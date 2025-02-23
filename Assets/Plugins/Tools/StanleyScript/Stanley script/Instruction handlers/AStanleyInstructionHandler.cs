using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public abstract class AStanleyInstructionHandler
		: IStanleyInstructionHandler
	{
		protected readonly ILoggerResolver loggerResolver;

		protected readonly ILogger logger;

		public AStanleyInstructionHandler(
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public abstract string Instruction { get; } //abstract to ensure the user WILL write this down

		public virtual string[] Aliases => Array.Empty<string>();

		#endregion

		#region Arguments

		public abstract int ArgumentsCount { get; } //abstract to ensure the user WILL write this down

		public abstract Type[] ArgumentTypes { get; } //abstract to ensure the user WILL write this down

		#endregion

		#region Return values

		public abstract int ReturnValuesCount { get; } //abstract to ensure the user WILL write this down

		public abstract Type[] ReturnValueTypes { get; } //abstract to ensure the user WILL write this down

		#endregion

		public virtual bool WillHandle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			return AssertInstructionHandlerSignature(
				context);
		}

		public abstract bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens);

		public virtual async Task<bool> Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return Handle(
				context,
				instruction,
				instructionTokens);
		}

		#endregion

		protected bool AssertInstructionHandlerSignature(
			IStanleyContextInternal context)
		{
			if (!AssertInstruction(
				Instruction))
				return false;

			var stack = context.StackMachine;

			int argumentsCount = ArgumentsCount;

			if (argumentsCount > 0)
			{
				if (!AssertMinStackSize(
					stack,
					argumentsCount))
					return false;
	
				for (int i = 0; i < argumentsCount; i++)
				{
					var currentArgumentType = ArgumentTypes[i];
	
					if (currentArgumentType == null)
						continue;
	
					if (!AssertStackVariableType(
						currentArgumentType,
						stack,
						i))
					{
						return false;
					}
				}
			}

			return true;
		}

		protected bool AssertInstruction(
			string instruction)
		{
			return instruction == Instruction;
		}

		protected bool AssertInstructionOrAlias(
			string instruction)
		{
			if (instruction == Instruction)
			{
				return true;
			}

			if (Aliases == null)
			{
				return false;
			}

			foreach (var alias in Aliases)
			{
				if (instruction == alias)
				{
					return true;
				}
			}

			return false;
		}

		protected bool AssertMinInstructionLength(
			string[] instructionTokens,
			int targetLength)
		{
			return instructionTokens.Length >= targetLength;
		}

		protected bool AssertInstructionNotEmpty(
			string[] instructionTokens,
			int instructionIndex)
		{
			return !string.IsNullOrEmpty(
				instructionTokens[instructionIndex]);
		}

		protected bool AssertMinStackSize(
			IStackMachine stack,
			int targetLength)
		{
			return stack.StackSize >= targetLength;
		}

		protected bool AssertStackVariableType<TVariable>(
			IStackMachine stack,
			int offsetFromTop)
		{
			if (!stack.PeekFromTop(
				offsetFromTop,
				out var variable))
			{
				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(typeof(TVariable)))
			{
				return false;
			}

			return true;
		}

		protected bool AssertStackVariableType(
			Type variableType,
			IStackMachine stack,
			int offsetFromTop)
		{
			if (!stack.PeekFromTop(
				offsetFromTop,
				out var variable))
			{
				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(variableType))
			{
				return false;
			}

			return true;
		}

		protected bool AssertStackVariableValue<TVariable>(
			TVariable expectedValue,
			IStackMachine stack,
			int offsetFromTop)
		{
			if (!stack.PeekFromTop(
				offsetFromTop,
				out var variable))
			{
				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(typeof(TVariable)))
			{
				return false;
			}

			if (!variable.GetValue<TVariable>().Equals(expectedValue))
			{
				return false;
			}

			return true;
		}

		protected ILogger SelectLogger(
			IStanleyContextInternal context)
		{
			return context.ReportMaker.ReportLogger ?? logger;
		}

		protected bool AssertVariable(
			IStanleyVariable variable,
			ILogger currentLogger)
		{
			if (variable == null)
			{
				currentLogger?.LogError(
					GetType(),
					"INVALID STACK VARIABLE");

				return false;
			}

			return true;
		}

		protected bool AssertVariableType<TVariable>(
			IStanleyVariable variable,
			ILogger currentLogger)
		{
			if (variable == null)
			{
				currentLogger?.LogError(
					GetType(),
					"INVALID STACK VARIABLE");

				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(typeof(TVariable)))
			{
				currentLogger?.LogError(
					GetType(),
					$"INVALID STACK VARIABLE TYPE. EXPECTED: {nameof(TVariable)} ACTUAL: {variable.VariableType.Name}");

				return false;
			}

			return true;
		}

		protected bool AssertVariableType(
			Type variableType,
			IStanleyVariable variable,
			ILogger currentLogger)
		{
			if (variable == null)
			{
				currentLogger?.LogError(
					GetType(),
					"INVALID STACK VARIABLE");

				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(variableType))
			{
				currentLogger?.LogError(
					GetType(),
					$"INVALID STACK VARIABLE TYPE. EXPECTED: {variableType.Name} ACTUAL: {variable.VariableType.Name}");

				return false;
			}

			return true;
		}

		protected bool AssertVariableTypeAndValue<TVariable>(
			IStanleyVariable variable,
			TVariable expectedValue,
			ILogger currentLogger)
		{
			if (variable == null)
			{
				currentLogger?.LogError(
					GetType(),
					"INVALID STACK VARIABLE");

				return false;
			}

			if (!variable.VariableType.IsSameOrInheritor(typeof(TVariable)))
			{
				currentLogger?.LogError(
					GetType(),
					$"INVALID STACK VARIABLE TYPE. EXPECTED: {nameof(TVariable)} ACTUAL: {variable.VariableType.Name}");

				return false;
			}

			if (!variable.GetValue<TVariable>().Equals(expectedValue))
			{
				currentLogger?.LogError(
					GetType(),
					$"INVALID STACK VARIABLE VALUE. EXPECTED: {expectedValue} ACTUAL: {variable.GetValue()}");

				return false;
			}

			return true;
		}

		protected bool AssertValueNotEmpty(
			string value,
			ILogger currentLogger)
		{
			if (string.IsNullOrEmpty(value))
			{
				currentLogger?.Log(
					GetType(),
					"INVALID VARIABLE VALUE");

				return false;
			}

			return true;
		}
	}
}