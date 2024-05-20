using System;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ButtonView : AView
    {
        protected string commandID;
        
        protected Button button;

        protected CommandDelegate onClickCommand = null;
        
        public ButtonView(
            IViewModel viewModel,
            string commandID,
            Button button,
            ILogger logger = null)
            : base(
                viewModel,
                logger)
        {
            this.commandID = commandID;

            this.button = button;
        }

        protected override void InitializeInternal(object[] args)
        {
            onClickCommand = viewModel.GetCommand(commandID);

            if (onClickCommand == null)
                throw new Exception(
                    logger.TryFormat<ButtonView>(
                        $"Could not obtain command \"{commandID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);
        }

        protected void OnButtonClicked(ClickEvent @event)
        {
            onClickCommand();
        }

        protected override void CleanupInternal()
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);

            base.CleanupInternal();
        }
    }
}