using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    [Component("Simulation world/Spawners")]
    public struct ProvideAuthoringPermissionComponent
    {
        public int PlayerSlot;
    }
}