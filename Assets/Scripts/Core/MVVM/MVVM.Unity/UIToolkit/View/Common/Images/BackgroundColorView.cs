using System;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class BackgroundColorView : AView
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

        protected override void InitializeInternal(object[] args)
        {
            if (!viewModel.GetObservable<Color>(
                propertyID,
                out colorProperty))
                throw new Exception(
                    logger.TryFormat<BackgroundColorView>(
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            colorProperty.OnValueChanged += OnColorChanged;

            OnColorChanged(colorProperty.Value);
        }
        
        protected void OnColorChanged(Color newValue)
        {
            visualElement.style.backgroundColor = newValue;
        }

        protected override void CleanupInternal()
        {
            if (colorProperty != null)
            {
                colorProperty.OnValueChanged -= OnColorChanged;

                colorProperty = null;
            }

            base.CleanupInternal();
        }
    }
}