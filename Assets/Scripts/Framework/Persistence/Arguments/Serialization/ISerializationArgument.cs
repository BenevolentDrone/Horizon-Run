namespace HereticalSolutions.Persistence
{
    public interface ISerializationArgument
	{
		string FullPath { get; }

		bool Append { get; }
	}
}