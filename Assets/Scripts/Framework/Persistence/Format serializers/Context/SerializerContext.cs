using System;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public class SerializerContext
		: ISerializerContext
	{
		public IVisitor Visitor { get; set; }

		public IFormatSerializer FormatSerializer { get; set; }

		public ISerializationStrategy SerializationStrategy { get; set; }

		public IStronglyTypedMetadata Arguments { get; set; }
	}
}