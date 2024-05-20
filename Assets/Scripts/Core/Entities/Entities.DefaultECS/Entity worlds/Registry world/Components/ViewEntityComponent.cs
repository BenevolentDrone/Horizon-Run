using DefaultEcs;

namespace HereticalSolutions.Entities
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