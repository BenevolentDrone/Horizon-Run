using System;

namespace HereticalSolutions.LifetimeManagement
{
    public interface ILifetimeable
    {
        #region Set up

        bool IsSetUp { get; }

        Action<ILifetimeable> OnSetUp { get; set; }
        
        #endregion

        #region Initialize

        bool IsInitialized { get; }

        Action<ILifetimeable, object[]> OnInitialized { get; set; }
        
        EInitializationFlags InitializationFlags { get; }

        #endregion

        #region Cleanup

        Action<ILifetimeable> OnCleanedUp { get; set; }

        #endregion

        #region Tear down

        Action<ILifetimeable> OnTornDown { get; set; }

        #endregion
    }
}