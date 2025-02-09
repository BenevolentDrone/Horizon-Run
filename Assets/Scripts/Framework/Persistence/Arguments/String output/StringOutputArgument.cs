namespace HereticalSolutions.Persistence
{
	[SerializationArgument]
	public class StringOutputArgument
		: IStringOutputArgument
	{
		public string Output { get; set; }

		public StringOutputArgument()
		{
			Output = string.Empty;
		}
	}
}