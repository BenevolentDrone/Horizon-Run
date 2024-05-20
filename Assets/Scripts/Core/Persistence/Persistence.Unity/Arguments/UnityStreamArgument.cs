using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Arguments
{
    public class UnityStreamArgument : ISerializationArgument
    {
        public UnityPersistentFilePathSettings Settings { get; set; }
    }
}