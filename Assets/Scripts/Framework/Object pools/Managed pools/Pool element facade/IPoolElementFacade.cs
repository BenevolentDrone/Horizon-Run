namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an element in a pool.
    /// </summary>
    /// <typeparam name="T">The type of the pool element.</typeparam>
    public interface IPoolElementFacade<T>
    {
        /// <summary>
        /// Gets or sets the value of the pool element.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Gets the status of the pool element.
        /// </summary>
        EPoolElementStatus Status { get; set; }

        IManagedPool<T> Pool { get; set; }

        /// <summary>
        /// Pushes the pool element.
        /// </summary>
        void Push();
    }
}