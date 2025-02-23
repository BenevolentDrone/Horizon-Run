using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyValueVariable
		: IStanleyVariable,
		  IClonable
	{
		private readonly string name;

		private readonly Type variableType;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		private object value;

		public StanleyValueVariable(
			string name,
			Type variableType,
			object value,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.name = name;

			this.variableType = variableType;

			this.value = value;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IStanleyVariable

		public string Name => name;

		public Type VariableType => variableType;

		public object GetValue()
		{
			return value;
		}

		public T GetValue<T>()
		{
			switch (value)
			{
				case T genericTypeValue:
					return genericTypeValue;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"COULD NOT CAST TYPE {value.GetType().Name} TO TYPE {nameof(T)}"));
			}
		}

		public void SetValue(
			object value)
		{
			if (value != null
				&& value.GetType() != variableType)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VARIABLE TYPE. EXPECTED: {VariableType.Name}, RECEIVED: {value.GetType().Name}"));
			}
			
			this.value = value;
		}

		public void SetValue<T>(
			T value)
		{
			if (value != null
				&& typeof(T) != variableType)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VARIABLE TYPE. EXPECTED: {VariableType.Name}, RECEIVED: {nameof(T)}"));
			}

			this.value = value;
		}

		#endregion

		#region IClonable

		public object Clone()
		{
			object clonedValue = (value is IClonable clonableValue)
				? clonableValue.Clone()
				: value;

			return StanleyFactory.BuildValueVariable(
				name,
				variableType,
				clonedValue,
				loggerResolver);
		}

		#endregion
	}
}