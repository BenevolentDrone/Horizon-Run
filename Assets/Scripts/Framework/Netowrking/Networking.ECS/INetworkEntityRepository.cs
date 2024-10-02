namespace HereticalSolutions.Networking.ECS
{
    public interface INetworkEntityRepository<TEntityID>
    {
        bool TryAllocateNetworkEntityID(
            out ushort networkID);
        
        bool HasNetworkEntity(
            ushort networkID);

        bool TryAddNetworkEntity(
            ushort networkID,
            TEntityID entityID);
        
        bool TryRemoveNetworkEntity(
            ushort networkID);
        
        bool TryGetEntityID(
            ushort networkID,
            out TEntityID entityID);
        
        bool TryGetNetworkID(
            TEntityID entityID,
            out ushort networkID);
    }
}