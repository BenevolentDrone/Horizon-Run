using System;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class CursorSwitcherPresenterSystem : AEntitySetSystem<float>
    {
        private readonly UniversalTemplateEntityManager entityManager;
        
        public CursorSwitcherPresenterSystem(
            World world,
            UniversalTemplateEntityManager entityManager)
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