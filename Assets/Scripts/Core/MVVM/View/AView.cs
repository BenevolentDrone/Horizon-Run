using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.View
{
    /// <summary>
    /// Represents a base class for views in the MVVM architecture.
    /// </summary>
    public abstract class AView
        : ALifetimeable,
          IInitializable
    {
        protected readonly IViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model associated with this view.</param>
        /// <param name="logger">The logger to be used for logging.</param>
        public AView(
            IViewModel viewModel,
            ILogger logger = null)
            : base(logger)
        {
            this.viewModel = viewModel;
        }

        #region IInitializable

        public virtual void Initialize(object[] args)
        {
            if (!IsSetUp)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "VIEW SHOULD BE SET UP BEFORE BEING INITIALIZED"));
            }

            if (IsInitialized)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "INITIALIZING VIEW THAT IS ALREADY INITIALIZED"));
            }

            InitializeInternal(args);

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        #endregion

        protected abstract void InitializeInternal(object[] args);

        protected void TryObtainProperty<T>(string propertyID, out IObservableProperty<T> property)
        {
            bool propertyObtained = viewModel.GetObservable<T>(propertyID, out property);

            if (!propertyObtained)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"COULD NOT OBTAIN PROPERTY {propertyID} FROM VIEWMODEL {viewModel.GetType().Name}"));
            }
        }
    }
}