using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [Component("View world/Presenters")]
    public struct QuaternionPresenterComponent
    {
        public Entity TargetEntity;
    }
}