using HereticalSolutions.Entities;

namespace HereticalSolutions.HorizonRun
{
    [Component("Simulation world/Sensors")]
    [ClientDisabledComponent]
    public struct SensorComponent
    {
        public ushort EntityListHandle;
    }
}