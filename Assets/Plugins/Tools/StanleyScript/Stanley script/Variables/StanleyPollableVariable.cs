using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyPollableVariable
		: IStanleyVariable,
		  IClonable
	{
		private readonly string name;

		private readonly Type variableType;

		private readonly Func<object> getValue;

		private readonly Action<object> setValue;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public StanleyPollableVariable(
			string name,
			Type variableType,
			Func<object> getValue,
			Action<object> setValue,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.name = name;

			this.variableType = variableType;
			
			this.getValue = getValue;

			this.setValue = setValue;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IStanleyVariable

		public string Name => name;

		public Type VariableType => variableType;

		public object GetValue()
		{
			return getValue?.Invoke();
		}

		public T GetValue<T>()
		{
			var result = getValue?.Invoke();

			switch (result)
			{
				case T value:
					return value;

				default:
					throw new InvalidCastException(
						logger.TryFormatException(
							GetType(),
							$"COULD NOT CAST TYPE {result.GetType().Name} TO TYPE {nameof(T)}"));
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

			setValue?.Invoke(value);
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

			setValue?.Invoke(value);
		}

		#endregion

		#region IClonable

		public object Clone()
		{
			return StanleyFactory.BuildPollableVariable(
				name,
				variableType,
				getValue,
				setValue,
				loggerResolver);
		}

		#endregion
	}
}