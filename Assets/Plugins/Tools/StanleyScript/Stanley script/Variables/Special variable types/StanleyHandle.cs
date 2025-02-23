namespace HereticalSolutions.StanleyScript
{
	public class StanleyHandle
		: IClonable
	{
		private readonly byte value;

		public byte Value => value;

		public StanleyHandle(
			byte handle)
		{
			this.value = handle;
		}

		#region IClonable

		public object Clone()
		{
			return StanleyFactory.BuildStanleyHandle(
				value);
		}

		#endregion
	}
}