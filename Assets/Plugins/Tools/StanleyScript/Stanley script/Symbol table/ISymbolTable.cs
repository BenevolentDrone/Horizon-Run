using System;

namespace HereticalSolutions.StanleyScript
{
	public interface ISymbolTable
		: IReadOnlySymbolTable
	{
		public bool TryAddVariable(
			IStanleyVariable variable);

		public void AddOrUpdateVariable(
			IStanleyVariable variable);

		public bool TryRemoveVariable(
			string variableName);
	}
}