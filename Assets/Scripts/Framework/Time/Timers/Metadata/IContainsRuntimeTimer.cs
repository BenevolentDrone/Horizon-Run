using HereticalSolutions.Delegates.Subscriptions;
using HereticalSolutions.Time;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents an interface for an object that contains a runtime timer.
    /// </summary>
    public interface IContainsRuntimeTimer
    {
        IRuntimeTimer RuntimeTimer { get; set; }
    }
}