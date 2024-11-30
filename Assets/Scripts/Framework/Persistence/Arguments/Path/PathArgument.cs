namespace HereticalSolutions.Persistence
{
    public class PathArgument
        : IPathArgument
    {
        public string Path { get; set; }

        public PathArgument()
        {
            Path = string.Empty;
        }
    }
}