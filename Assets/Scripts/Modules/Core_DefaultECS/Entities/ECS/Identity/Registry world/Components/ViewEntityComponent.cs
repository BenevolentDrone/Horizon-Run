using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [Component("View world")]
    [WorldIdentityComponent]
    public struct ViewEntityComponent
    {
        /// <summary>
        /// The view entity associated with this component.
        /// </summary>
        public Entity ViewEntity;

        /// <summary>
        /// The prototype ID of the entity.
        /// </summary>
        public string PrototypeID;
    }
}