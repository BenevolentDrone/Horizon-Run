using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.View
{
    /// <summary>
    /// Represents a base class for views in the MVVM architecture.
    /// </summary>
    public abstract class AView
    {
        protected readonly IViewModel viewModel;

        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model associated with this view.</param>
        /// <param name="logger">The logger to be used for logging.</param>
        public AView(
            IViewModel viewModel,
            ILogger logger = null)
        {
            this.viewModel = viewModel;
            
            this.logger = logger;
        }

        protected void TryObtainProperty<T>(string propertyID, out IObservableProperty<T> property)
        {
            bool propertyObtained = viewModel.GetObservable<T>(propertyID, out property);

            if (!propertyObtained)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"COULD NOT OBTAIN PROPERTY {propertyID} FROM VIEWMODEL {viewModel.GetType().Name}"));
            }
        }
    }
}