using System.IO;

namespace HereticalSolutions.Persistence
{
	[SerializationArgument]
	public class SourceStreamArgument
		: ISourceStreamArgument
	{
		public Stream Source { get; set; }

		public SourceStreamArgument()
		{
			Source = null;
		}
	}
}