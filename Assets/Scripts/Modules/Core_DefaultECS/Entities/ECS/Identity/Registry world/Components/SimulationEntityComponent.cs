using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    /// <summary>
    /// Component representing a simulation entity in the game.
    /// </summary>
    [Component("Simulation world")]
    [WorldIdentityComponent]
    public struct SimulationEntityComponent 
    {
        /// <summary>
        /// The simulation entity.
        /// </summary>
        public Entity SimulationEntity;

        /// <summary>
        /// The prototype ID.
        /// </summary>
        public string PrototypeID;
    }
}