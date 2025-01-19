using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleView
        : AView,
          IInitializable,
          ICleanuppable
    {
        protected string propertyID;
        
        protected Toggle toggle;

        protected IObservableProperty<bool> boolProperty = null;

        protected bool togglePressed = false;
        
        public ToggleView(
            IViewModel viewModel,
            string propertyID,
            Toggle toggle,
            ILogger logger)
            : base(
                viewModel,
                logger)
        {
            this.propertyID = propertyID;

            this.toggle = toggle;
        }

        public bool Initialize(object[] args)
        {
            if (!viewModel.GetObservable<bool>(
                propertyID,
                out boolProperty))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));

            boolProperty.OnValueChanged += OnBoolChanged;

            OnBoolChanged(boolProperty.Value);
            
            toggle.RegisterValueChangedCallback(OnToggleValueChanged);

            return true;
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

        public void Cleanup()
        {
            togglePressed = false;
            
            if (boolProperty != null)
            {
                boolProperty.OnValueChanged -= OnBoolChanged;

                boolProperty = null;
            }
            
            toggle.UnregisterValueChangedCallback(OnToggleValueChanged);
        }
    }
}