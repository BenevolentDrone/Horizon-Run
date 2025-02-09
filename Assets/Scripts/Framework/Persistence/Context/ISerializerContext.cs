using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public interface ISerializerContext
		: IReadOnlySerializerContext
	{
		new IVisitor Visitor { get; set; }

		new IFormatSerializer FormatSerializer { get; set; }

		new IDataConverter DataConverter { get; set; }

		new ISerializationStrategy SerializationStrategy { get; set; }

		new IStronglyTypedMetadata Arguments { get; set; }
	}
}