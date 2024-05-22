using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
    [Component("View world/Presenters")]
    public struct CursorSwitcherPresenterComponent
    {
        public Entity TargetEntity;
    }
}