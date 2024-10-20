using System;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class CursorSwitcherPresenterSystem : AEntitySetSystem<float>
    {
        private readonly EntityManager entityManager;
        
        public CursorSwitcherPresenterSystem(
            World world,
            EntityManager entityManager)
            : base(
                world
                    .GetEntities()
                    .With<CursorSwitcherPresenterComponent>()
                    .With<CursorSwitcherViewComponent>()
                    .AsSet())
        {
            //TODO
            throw new NotImplementedException();

            this.entityManager = entityManager;
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