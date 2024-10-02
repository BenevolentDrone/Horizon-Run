namespace HereticalSolutions.Networking.ECS
{
    public interface INetworkPrototypeEntityRepository
    {
        void RegisterPrototype(string prototypeID);
        
        bool TryGetUShortPrototypeID(
            string prototypeID,
            out ushort ushortPrototypeID);

        bool TryGetStringPrototypeID(
            ushort prototypeID,
            out string stringPrototypeID);
    }
}