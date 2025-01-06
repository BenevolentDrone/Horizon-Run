namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents a repository for cloning objects.
    /// </summary>
    public interface ICloneableInstanceRepository
    {
        /// <summary>
        /// Creates a clone of the object repository.
        /// </summary>
        /// <returns>A clone of the object repository.</returns>
        IInstanceRepository Clone();
    }
}