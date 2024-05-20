using System;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyCachedVariable
		: IStanleyVariable
	{
		private object value;

		public StanleyCachedVariable(
			string name,
			Type variableType,
			object value)
		{
			Name = name;

			VariableType = variableType;

			this.value = value;
		}

		#region IStanleyVariable

		public string Name { get; private set; }

		public Type VariableType { get; private set; }

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
					throw new Exception($"[StanleyCachedVariable] COULD NOT CAST TYPE {value.GetType().Name} TO TYPE {typeof(T).Name}");
			}
		}

		#endregion
	}
}