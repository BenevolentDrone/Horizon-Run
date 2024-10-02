using System.Numerics;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    /// <summary>
    /// Network event component for storing target position data.
    /// </summary>
    [NetworkEventComponent]
    public struct EventTargetPositionComponent
    {
        /// <summary>
        /// The target position in 3D space.
        /// </summary>
        public Vector3 Position;
    }
}