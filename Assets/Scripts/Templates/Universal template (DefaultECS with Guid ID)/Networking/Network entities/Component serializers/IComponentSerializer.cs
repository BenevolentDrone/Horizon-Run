using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
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