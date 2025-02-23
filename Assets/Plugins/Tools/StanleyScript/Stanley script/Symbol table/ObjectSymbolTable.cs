using System.Collections.Generic;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.StanleyScript
{
	public class ObjectSymbolTable
		: ISymbolTable
	{
		private readonly IRepository<string, IStanleyVariable> properties;

		public ObjectSymbolTable(
			IRepository<string, IStanleyVariable> properties)
		{
			this.properties = properties;
		}

		#region ISymbolTable

		#region IReadOnlySymbolTable

		public bool TryGetVariable(
			string name,
			out IStanleyVariable variable)
		{
			return properties.TryGet(
				name,
				out variable);
		}

		public IEnumerable<IStanleyVariable> AllVariables => properties.Values;

		#endregion

		public bool TryAddVariable(
			IStanleyVariable variable)
		{
			return properties.TryAdd(
				variable.Name,
				variable);
		}

		public void AddOrUpdateVariable(
			IStanleyVariable variable)
		{
			properties.AddOrUpdate(
				variable.Name,
				variable);
		}

		public bool TryRemoveVariable(
			string variableName)
		{
			return properties.TryRemove(
				variableName);
		}

		#endregion

		#region IClonable

		public object Clone()
		{
			var result = StanleyFactory.BuildObjectSymbolTable();

			foreach (var variable in AllVariables)
			{
				var clonableVariable = variable as IClonable;

				result.TryAddVariable(
					(IStanleyVariable)clonableVariable.Clone());
			}

			return result;
		}

		#endregion
	}
}