namespace HereticalSolutions.Entities
{
    public interface ISubaddressManager
    {
        void MemorizeSubaddressPart(string subaddressPart);
        
        bool TryGetSubaddressPart(
            string subaddressPart,
            out ushort subaddressPartIndex);
        
        bool TryGetSubaddressPart(
            ushort subaddressPartIndex,
            out string subaddressPart);
    }
}