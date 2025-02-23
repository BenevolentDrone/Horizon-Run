using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
	public interface IReadOnlySymbolTable
	{
		public bool TryGetVariable(
			string name,
			out IStanleyVariable variable);
			
		public IEnumerable<IStanleyVariable> AllVariables { get; }
	}
}