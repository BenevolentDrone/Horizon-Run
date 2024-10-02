using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    [Component("Simulation world/Sensors")]
    [ClientDisabledComponent]
    public struct SensorComponent
    {
        public ushort EntityListHandle;
    }
}