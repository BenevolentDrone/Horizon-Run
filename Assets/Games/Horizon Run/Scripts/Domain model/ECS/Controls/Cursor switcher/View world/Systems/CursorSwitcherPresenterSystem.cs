using System;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
    public class CursorSwitcherPresenterSystem : AEntitySetSystem<float>
    {
        private readonly HorizonRunEntityManager entityManager;
        
        public CursorSwitcherPresenterSystem(
            World world,
            HorizonRunEntityManager entityManager)
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