using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class PlaceholderInitializationSystem : ISystem<Entity>
    {
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity state)
        {
        }

        public void Dispose()
        {
        }
    }
}