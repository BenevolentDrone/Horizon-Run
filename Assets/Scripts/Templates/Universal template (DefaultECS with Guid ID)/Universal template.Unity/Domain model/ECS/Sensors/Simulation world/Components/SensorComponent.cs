using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [Component("Simulation world/Sensors")]
    [ClientDisabledComponent]
    public struct SensorComponent
    {
        public ushort EntityListHandle;
    }
}