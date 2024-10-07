using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public interface IContainsComponentData
        : INetSerializable
    {
        IComponentSerializerManager ComponentSerializerManager { get; set; }
    }
}