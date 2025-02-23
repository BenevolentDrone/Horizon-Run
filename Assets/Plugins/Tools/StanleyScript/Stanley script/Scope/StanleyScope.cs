namespace HereticalSolutions.StanleyScript
{
	public class StanleyScope
	{
		private readonly StanleyHandle handle;

		public StanleyHandle Handle => handle;

		private readonly StanleyScope parentScope;

		public StanleyScope ParentScope => parentScope;

		private readonly ISymbolTable variables;

		public ISymbolTable Variables => variables;

		public StanleyScope(
			StanleyHandle handle,
			StanleyScope parentScope,
			ISymbolTable variables)
		{
			this.handle = handle;

			this.parentScope = parentScope;

			this.variables = variables;
		}
	}
}