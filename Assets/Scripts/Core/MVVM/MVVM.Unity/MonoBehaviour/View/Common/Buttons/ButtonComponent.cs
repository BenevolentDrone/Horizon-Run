using UnityEngine;
using UnityEngine.UI;

using HereticalSolutions.Logging;

using ILogger = HereticalSolutions.Logging.ILogger;

using Zenject;

namespace HereticalSolutions.MVVM.Mono
{
    public class ButtonComponent : AViewComponent
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [SerializeField]
        protected string commandID;

        [SerializeField]
        private Button button;

        private ILogger logger;

        protected override void Awake()
        {
            logger = loggerResolver.GetLogger<ButtonComponent>();

            view = new ButtonView(
                baseViewModel.ViewModel,
                commandID,
                button,
                logger);
            
            base.Awake();
        }
    }
}