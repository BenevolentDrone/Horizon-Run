using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleButtonView
        : AView,
          IInitializable,
          ICleanuppable
    {
        protected string propertyID;
        
        protected Button button;

        protected IObservableProperty<bool> boolProperty = null;
        
        public ToggleButtonView(
            IViewModel viewModel,
            string propertyID,
            Button button,
            ILogger logger = null)
            : base(
                viewModel,
                logger)
        {
            this.propertyID = propertyID;

            this.button = button;
        }

        public bool Initialize(object[] args)
        {
            if (!viewModel.GetObservable<bool>(
                propertyID,
                out boolProperty))
                throw new Exception(
                    logger.TryFormatException<ToggleButtonView>(
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            return true;
        }

        protected void OnButtonClicked(ClickEvent @event)
        {
            boolProperty.Value = !boolProperty.Value;
        }

        public void Cleanup()
        {
            if (boolProperty != null)
            {
                boolProperty = null;
            }
            
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }
}