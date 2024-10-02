using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    [Component("View world/Presenters")]
    public struct ServerPositionRotation2DPresenterComponent
    {
        public Entity TargetEntity;
    }
}