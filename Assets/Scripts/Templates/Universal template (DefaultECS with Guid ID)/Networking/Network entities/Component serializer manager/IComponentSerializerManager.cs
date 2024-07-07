using DefaultEcs;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public interface IComponentSerializerManager
    {
        ESerializedEntityType SerializedEntityType { get; }

        ComponentSerializationContext Context { get; }
        
        void Serialize(
            IContainsComponentData packet,
            NetDataWriter writer);

        void InvokeComponentSerializersSerialization(
            IComponentSerializer[] serializers,
            ushort[] serializerIDs,
            NetDataWriter writer);
        
        void Deserialize(
            IContainsComponentData packet,
            NetDataReader reader);

        void InvokeComponentSerializersDeserialization(
            IComponentSerializer[] serializers,
            ushort[] serializerIDs,
            NetDataReader reader);

        ushort ParseEntity(
            Entity entity,
            out ushort[] serializerIDs,
            out IComponentSerializer[] serializers);

        void PopulateEntity(
            Entity entity,
            IComponentSerializer[] serializers,
            ushort[] serializerIDs);
    }
}