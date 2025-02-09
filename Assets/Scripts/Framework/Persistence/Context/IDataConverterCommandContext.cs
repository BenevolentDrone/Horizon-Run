using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public interface IDataConverterCommandContext
	{
		ISerializationStrategy SerializationStrategy { get; }

		IStronglyTypedMetadata Arguments { get; }
	}
}