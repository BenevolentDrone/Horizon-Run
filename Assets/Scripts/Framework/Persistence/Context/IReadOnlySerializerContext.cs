namespace HereticalSolutions.Persistence
{
	public interface IReadOnlySerializerContext
		: ISerializationCommandContext
	{
		IVisitor Visitor { get; }

		IFormatSerializer FormatSerializer { get; }
	}
}