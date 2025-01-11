using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class BackgroundColorView
        : AView,
          IInitializable,
          ICleanuppable
    {
        protected string propertyID;
        
        protected VisualElement visualElement;

        protected IObservableProperty<Color> colorProperty = null;

        public BackgroundColorView(
            IViewModel viewModel,
            string propertyID,
            VisualElement visualElement,
            ILogger logger = null)
            : base(
                viewModel,
                logger)
        {
            this.propertyID = propertyID;

            this.visualElement = visualElement;
        }

        public bool Initialize(object[] args)
        {
            if (!viewModel.GetObservable<Color>(
                propertyID,
                out colorProperty))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            colorProperty.OnValueChanged += OnColorChanged;

            OnColorChanged(colorProperty.Value);

            return true;
        }
        
        protected void OnColorChanged(Color newValue)
        {
            visualElement.style.backgroundColor = newValue;
        }

        public void Cleanup()
        {
            if (colorProperty != null)
            {
                colorProperty.OnValueChanged -= OnColorChanged;

                colorProperty = null;
            }
        }
    }
}