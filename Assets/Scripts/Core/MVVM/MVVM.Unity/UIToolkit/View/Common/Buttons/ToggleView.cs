using System;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleView
        : AView
    {
        protected string propertyID;
        
        protected Toggle toggle;

        protected IObservableProperty<bool> boolProperty = null;

        protected bool togglePressed = false;
        
        public ToggleView(
            IViewModel viewModel,
            string propertyID,
            Toggle toggle,
            ILogger logger = null)
            : base(
                viewModel,
                logger)
        {
            this.propertyID = propertyID;

            this.toggle = toggle;
        }

        protected override void InitializeInternal(object[] args)
        {
            if (!viewModel.GetObservable<bool>(
                propertyID,
                out boolProperty))
                throw new Exception(
                    logger.TryFormat<ToggleView>(
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));

            boolProperty.OnValueChanged += OnBoolChanged;

            OnBoolChanged(boolProperty.Value);
            
            toggle.RegisterValueChangedCallback(OnToggleValueChanged);
        }
        
        protected virtual void OnBoolChanged(bool newValue)
        {
            if (togglePressed)
                return;
            
            toggle.value = newValue;
        }
        
        protected void OnToggleValueChanged(ChangeEvent<bool> @event)
        {
            togglePressed = true;
            
            boolProperty.Value = @event.newValue;

            togglePressed = false;
        }

        protected override void CleanupInternal()
        {
            togglePressed = false;
            
            if (boolProperty != null)
            {
                boolProperty.OnValueChanged -= OnBoolChanged;

                boolProperty = null;
            }
            
            toggle.UnregisterValueChangedCallback(OnToggleValueChanged);

            base.CleanupInternal();
        }
    }
}