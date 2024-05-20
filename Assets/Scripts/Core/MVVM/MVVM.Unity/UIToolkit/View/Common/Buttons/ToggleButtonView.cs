using System;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleButtonView
        : AView
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

        protected override void InitializeInternal(object[] args)
        {
            if (!viewModel.GetObservable<bool>(
                propertyID,
                out boolProperty))
                throw new Exception(
                    logger.TryFormat<ToggleButtonView>(
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);
        }

        protected void OnButtonClicked(ClickEvent @event)
        {
            boolProperty.Value = !boolProperty.Value;
        }

        protected override void CleanupInternal()
        {
            if (boolProperty != null)
            {
                boolProperty = null;
            }
            
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);

            base.CleanupInternal();
        }
    }
}