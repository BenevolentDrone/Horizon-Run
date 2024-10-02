using HereticalSolutions.Collections;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools
{
    public class PoolElementFacadeWithArrayIndex<T>
        : PoolElementFacade<T>,
          IIndexed
    {
        public PoolElementFacadeWithArrayIndex(
            IReadOnlyObjectRepository metadata)
            : base (metadata)
        {
        }

        #region IIndexed

        public int Index { get; set; }

        #endregion

        #region ICleanUppable

        public override void Cleanup()
        {
            Index = -1;
            
            base.Cleanup();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Cleanup();
        }

        #endregion
    }
}