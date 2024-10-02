using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class PlaceholderSystem : ISystem<float>
    {
        public bool IsEnabled { get; set; } = true;

        public void Update(float state)
        {
        }

        public void Dispose()
        {
        }
    }
}