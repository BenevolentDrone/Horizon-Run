using System;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public interface IReadOnlySerializerContext
	{
		IVisitor Visitor { get; }

		IFormatSerializer FormatSerializer { get; }

		ISerializationStrategy SerializationStrategy { get; }

		IStronglyTypedMetadata Arguments { get; }
	}
}