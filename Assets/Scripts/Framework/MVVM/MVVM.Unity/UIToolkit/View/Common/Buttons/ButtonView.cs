using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ButtonView
        : AView,
          IInitializable,
          ICleanuppable
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

        public bool Initialize(object[] args)
        {
            onClickCommand = viewModel.GetCommand(commandID);

            if (onClickCommand == null)
                throw new Exception(
                    logger.TryFormatException<ButtonView>(
                        $"Could not obtain command \"{commandID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            return true;
        }

        protected void OnButtonClicked(ClickEvent @event)
        {
            onClickCommand();
        }

        public void Cleanup()
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }
}