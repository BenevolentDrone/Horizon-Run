using System;

namespace HereticalSolutions.Persistence
{
	[SerializationArgument]
	public class SourceByteArrayArgument
		: ISourceByteArrayArgument
	{
		public byte[] Source { get; set; }

		public SourceByteArrayArgument()
		{
			Source = Array.Empty<byte>();
		}
	}
}