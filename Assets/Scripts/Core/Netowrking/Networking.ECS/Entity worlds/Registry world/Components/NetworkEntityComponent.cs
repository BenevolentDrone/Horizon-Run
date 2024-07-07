using HereticalSolutions.Entities;

namespace HereticalSolutions.Networking.ECS
{
    [Component("Networking")]
    public struct NetworkEntityComponent
    {
        public ushort NetworkID;

        public ushort LastReceivedServerTick;
    }
}