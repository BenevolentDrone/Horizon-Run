using System;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class CursorSwitcherModelValidationSystem : AEntitySetSystem<float>
    {
        private EntitySet selectionSet;

        public CursorSwitcherModelValidationSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<CursorSwitcherPresenterComponent>()
                    .AsSet())
        {
            //TODO
            throw new NotImplementedException();
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}