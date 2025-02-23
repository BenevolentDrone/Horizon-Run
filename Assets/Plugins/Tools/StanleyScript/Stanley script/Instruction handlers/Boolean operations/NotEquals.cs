using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class NotEquals
		: AStanleyInstructionHandler
	{
		public NotEquals(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_NEQ;

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

			if (!stack.Pop(
				out var variable2))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			var value1 = variable1.GetValue();

			var value2 = variable2.GetValue();
			
			bool result;
			
			if (variable1.VariableType != variable2.VariableType)
			{
				//As a backup plan, compare floats
				if (stack.CanCast(
					    variable1.VariableType,
					    typeof(float))
				    && stack.CanCast(
					    variable2.VariableType,
					    typeof(float)))
				{
					if (!stack.TryCast(
						    variable1.GetValue(),
						    typeof(float),
						    out value1))
					{
						currentLogger?.LogError(
							GetType(),
							$"COULD NOT COMPARE TYPES: {variable1.VariableType.Name} AND {variable2.VariableType.Name}");

						return false;
					}

					if (!stack.TryCast(
						    variable2.GetValue(),
						    typeof(float),
						    out value2))
					{
						currentLogger?.LogError(
							GetType(),
							$"COULD NOT COMPARE TYPES: {variable1.VariableType.Name} AND {variable2.VariableType.Name}");

						return false;
					}
					
					var float1 = value1.CastFromTo<object, float>();
            
					var float2 = value2.CastFromTo<object, float>();

					result = Math.Abs(float2 - float1) > MathHelpers.EPSILON;
			

					stack.Push(
						StanleyFactory.BuildValueVariable(
							StanleyConsts.RVALUE_VARIABLE_NAME,
							typeof(bool),
							result,
							loggerResolver));

					return true;
				}
				
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT COMPARE TYPES: {variable1.VariableType.Name} AND {variable2.VariableType.Name}");

				return false;
			}

			if (variable1.VariableType == typeof(string))
			{
				// Use string comparison for strings
				result = !string.Equals(
					value1 as string,
					value2 as string,
					StringComparison.Ordinal);
			}
			else if (variable1.VariableType == typeof(float))
			{
				result = 
					Math.Abs(
						value2.CastFromTo<object, float>()
						- value1.CastFromTo<object, float>())
					> MathHelpers.EPSILON;
			}
			else if (variable1.VariableType == typeof(StanleyHandle))
			{
				result = ((StanleyHandle)variable1).Value != ((StanleyHandle)variable2).Value;
			}
			else
			{
				// Use default Equals for other types
				result = !value1.Equals(value2);
			}

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