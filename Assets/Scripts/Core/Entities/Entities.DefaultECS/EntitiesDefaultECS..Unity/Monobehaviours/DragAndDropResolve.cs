using UnityEngine;

#if UNITY_EDITOR

using System;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Messaging;

using HereticalSolutions.Logging;

using Zenject;

#endif

namespace HereticalSolutions.Entities
{
	public class DragAndDropResolve
		: MonoBehaviour
	{
#if UNITY_EDITOR

        private CancellationTokenSource cancellationTokenSource;
        
#endif

		[SerializeField]
		private string prototypeID;

		public string PrototypeID
		{
			get => prototypeID;
		}

		private bool IsInitialized { get; set; } = false;


#if UNITY_EDITOR

		/*
        private void Start()
        {
            if(!IsInitialized)
            {
                Init();
                
                IsInitialized = true;
            }
        }

        private void Init()
        {
            var viewEntityAdapter = GetComponent<GameObjectViewEntityAdapter>();

            if (viewEntityAdapter.Initialized)
                return;

            var sceneEntity = GetComponent<SceneEntity>();

            if (sceneEntity != null)
                return;

            var ecsBusAsSender = ProjectContext
				.Instance
				.Container
				.ResolveId<INonAllocMessageSender>("ECSBus");

            var loggerResolver = ProjectContext
				.Instance
				.Container
				.Resolve<ILoggerResolver>();

			var logger = loggerResolver?.GetLogger<ResolveMeAs>();

            cancellationTokenSource = new CancellationTokenSource();

            var task =
                SendMessagesUntilResolvedTask(
                        ecsBusAsSender,
                        viewEntityAdapter,
                        cancellationTokenSource.Token)
                    .ThrowExceptions<ResolveMeAs>(logger);
        }


        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
        
            cancellationTokenSource?.Dispose();
        }

        private async Task SendMessagesUntilResolvedTask(
            INonAllocMessageSender ecsBusAsSender,
            GameObjectViewEntityAdapter adapter,
            CancellationToken cancellationToken)
        {
            while (!adapter.Initialized)
            {
                ecsBusAsSender
                    .PopMessage<ResolveRequestMessage>(out var message)
                    .Write<ResolveRequestMessage>(
                        message,
                        new object[]
                        {
                            gameObject,
                            prototypeID
                        })
                    .SendImmediately<ResolveRequestMessage>(message);

                await Task.Yield();
            }
        }
		*/
#endif
	}
}