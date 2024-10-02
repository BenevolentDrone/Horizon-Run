using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public interface IComponentSerializer
        : INetSerializable
    {
        ESerializedEntityType SerializedEntityType { get; }
        
        bool ReadFrom(
            Entity entity,
            ComponentSerializationContext context);

        bool WriteTo(
            Entity entity,
            ComponentSerializationContext context);

        void Clear();
    }
}