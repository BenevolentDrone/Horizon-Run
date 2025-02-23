using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ModuloDifferentTypes
		: AStanleyInstructionHandler
	{
		public ModuloDifferentTypes(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_MODULO;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			null,
			null
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(float)
		};

		#endregion

		public override bool WillHandle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			if (!AssertInstruction(
				    instruction))
				return false;

			var stack = context.StackMachine;

			if (!AssertMinStackSize(
				    stack,
				    2))
				return false;

			if (!stack.PeekFromTop(
				    0,
				    out var variable1))
			{
				return false;
			}

			if (!stack.PeekFromTop(
				    1,
				    out var variable2))
			{
				return false;
			}

			if (variable1.VariableType == variable2.VariableType)
			{
				return false;
			}

			if (!stack.CanCast(
				    variable1.VariableType,
				    typeof(float)))
			{
				return false;
			}

			if (!stack.CanCast(
				    variable2.VariableType,
				    typeof(float)))
			{
				return false;
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

			if (!stack.TryCast(
				    variable1.GetValue(),
				    typeof(float),
				    out var value1))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT CONVERT TO FLOAT: {variable1.VariableType.Name}");

				return false;
			}

			if (!stack.TryCast(
				    variable2.GetValue(),
				    typeof(float),
				    out var value2))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT CONVERT TO FLOAT: {variable2.VariableType.Name}");

				return false;
			}

			var float1 = value1.CastFromTo<object, float>();
            
			var float2 = value2.CastFromTo<object, float>();

			var result = float1 % float2;

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(float),
					result,
					loggerResolver));

			return true;
		}

		#endregion
	}
}