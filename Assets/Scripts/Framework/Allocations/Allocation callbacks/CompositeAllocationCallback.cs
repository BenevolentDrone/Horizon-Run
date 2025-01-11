using System.Collections.Generic;

namespace HereticalSolutions.Allocations
{
    public class CompositeAllocationCallback<T>
        : IAllocationCallback<T>
    {
        private readonly List<IAllocationCallback<T>> callbacks;

        public CompositeAllocationCallback(
            List<IAllocationCallback<T>> callbacks)
        {
            this.callbacks = callbacks;
        }

        #region IAllocationCallback

        public void OnAllocated(
            T element)
        {
            foreach (var callback in callbacks)
                callback.OnAllocated(element);
        }

        #endregion

        public void AddCallback(
            IAllocationCallback<T> callback)
        {
            callbacks.Add(callback);
        }
        
        public void RemoveCallback(
            IAllocationCallback<T> callback)
        {
            callbacks.Remove(callback);
        }
    }
}