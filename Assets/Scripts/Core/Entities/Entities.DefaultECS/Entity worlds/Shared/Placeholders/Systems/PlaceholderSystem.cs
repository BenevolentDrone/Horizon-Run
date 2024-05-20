using DefaultEcs.System;

namespace HereticalSolutions.Entities
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