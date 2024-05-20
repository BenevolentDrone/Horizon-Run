using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using HereticalSolutions.MVVM.View;

using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Mono
{
    public class ButtonView
        : AView
    {
        protected string commandID;

        protected Button button;
        
        protected CommandDelegate onClickCommand = null;
        
        public ButtonView(
            IViewModel viewModel,
            string commandID,
            Button button,
            ILogger logger)
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
            
            button.onClick.AddListener(OnButtonClicked);
        }
        
        protected void OnButtonClicked()
        {
            onClickCommand();
        }

        protected override void CleanupInternal()
        {
            button.onClick.RemoveAllListeners();

            base.CleanupInternal();
        }
    }
}