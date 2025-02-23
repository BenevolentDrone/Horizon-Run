namespace HereticalSolutions.StanleyScript
{
	public class StanleyObject
		: IClonable
	{
		protected readonly ISymbolTable properties;

		public ISymbolTable Properties => properties;

		public StanleyObject(
			ISymbolTable properties)
		{
			this.properties = properties;
		}

		#region IClonable

		public virtual object Clone()
		{
			var propertiesAsClonable = properties as IClonable;

			return StanleyFactory.BuildStanleyObject(
				(ISymbolTable)propertiesAsClonable.Clone());
		}

		#endregion
	}
}