using System;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class LabelView : AView
    {
        protected string propertyID;
        
        protected Label label;
        
        protected IObservableProperty<string> textProperty = null;
        
        public LabelView(
            IViewModel viewModel,
            string propertyID,
            Label label,
            ILogger logger = null)
            : base(
                viewModel,
                logger)
        {
            this.propertyID = propertyID;

            this.label = label;
        }

        protected override void InitializeInternal(object[] args)
        {
            if (!viewModel.GetObservable<string>(propertyID, out textProperty))
                throw new Exception(
                    logger.TryFormat<LabelView>(
                        $"Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\""));

            textProperty.OnValueChanged += OnTextChanged;

            OnTextChanged(textProperty.Value);
        }
        
        protected void OnTextChanged(string newValue)
        {
            label.text = string.IsNullOrEmpty(newValue)
                ? string.Empty
                : newValue;
        }

        protected override void CleanupInternal()
        {
            if (textProperty != null)
            {
                textProperty.OnValueChanged -= OnTextChanged;

                textProperty = null;
            }

            base.CleanupInternal();
        }
    }
}