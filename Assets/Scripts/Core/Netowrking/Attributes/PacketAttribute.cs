namespace HereticalSolutions.Networking
{
    [System.AttributeUsage(
        System.AttributeTargets.Class   //TODO: check whether we actually need classes for this
        | System.AttributeTargets.Struct)]
    public class PacketAttribute : System.Attribute
    {
        public PacketAttribute()
        {
        }
    }
}