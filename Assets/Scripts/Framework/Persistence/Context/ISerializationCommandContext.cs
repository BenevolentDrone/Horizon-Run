namespace HereticalSolutions.Persistence
{
	public interface ISerializationCommandContext
		: IDataConverterCommandContext
	{
		IDataConverter DataConverter { get; }
	}
}