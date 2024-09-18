using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM.View;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Mono
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
            ILogger logger)
            : base(
                viewModel,
                logger)
        {
            this.commandID = commandID;

            this.button = button;
        }

        #region IInitializable
        
        public bool Initialize(object[] args)
        {
            onClickCommand = viewModel.GetCommand(commandID);

            if (onClickCommand == null)
                throw new Exception(
                    logger.TryFormatException<ButtonView>(
                        $"Could not obtain command \"{commandID}\" from ViewModel \"{viewModel.GetType()}\""));
            
            button.onClick.AddListener(OnButtonClicked);

            return true;
        }
        
        #endregion
        
        protected void OnButtonClicked()
        {
            onClickCommand();
        }

        #region ICleanUppable

        public void Cleanup()
        {
            button.onClick.RemoveAllListeners();
        }
        
        #endregion
    }
}