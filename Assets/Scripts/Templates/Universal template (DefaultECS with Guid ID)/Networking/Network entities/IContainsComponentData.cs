using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public interface IContainsComponentData
        : INetSerializable
    {
        IComponentSerializerManager ComponentSerializerManager { get; set; }
    }
}