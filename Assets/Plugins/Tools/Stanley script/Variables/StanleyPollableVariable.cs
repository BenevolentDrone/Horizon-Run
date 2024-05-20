using System;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyPollableVariable
		: IStanleyVariable
	{
		private Func<object> getValue;

		public StanleyPollableVariable(
			string name,
			Type variableType,
			Func<object> getValue)
		{
			Name = name;

			VariableType = variableType;
			
			this.getValue = getValue;
		}

		#region IStanleyVariable

		public string Name { get; private set; }

		public Type VariableType { get; private set; }

		public object GetValue()
		{
			return getValue.Invoke();
		}

		public T GetValue<T>()
		{
			var result = GetValue();

			switch (result)
			{
				case T value:
					return value;

				default:
					throw new InvalidCastException();
			}
		}

		#endregion
	}
}