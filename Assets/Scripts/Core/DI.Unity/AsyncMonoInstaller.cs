using System.Text;
using System.Threading.Tasks;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.DI
{
	/// <summary>
	/// An abstract class that provides asynchronous installation functionality for Zenject mono installers.
	/// </summary>
	public abstract class AsyncMonoInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		protected AsyncMonoInstaller nextInstaller;

		protected ILogger logger;

		public AsyncMonoInstaller NextInstaller
		{
			get => nextInstaller;
		}

		private bool installed = false;

		public bool Installed
		{
			get => installed;
		}

		/// <summary>
		/// Installs the Zenject bindings asynchronously.
		/// </summary>
		public override void InstallBindings()
		{
			logger = loggerResolver.GetLogger(GetType());

			logger?.Log(
				GetType(),
				$"INSTALLING BINDINGS. INITIALIZING ASYNC INSTALL CHAIN");

			var _ =
				InstallAsync()
					.ThrowExceptions(
						GetType(),
						logger);
		}

		/// <summary>
		/// Installs Zenject bindings asynchronously.
		/// </summary>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		public async Task InstallAsync()
		{
			logger?.Log(
				GetType(),
				"INITIALIZING ASYNC INSTALL");

			await Install();

			installed = true;


			logger?.Log(
				GetType(),
				"ASYNC INSTALL FINISHED");

			if (nextInstaller != null)
			{
				Container.Inject(nextInstaller);

				var _ =
					nextInstaller
						.InstallAsync()
						.ThrowExceptions(
							GetType(),
							logger);
			}
		}

		/// <summary>
		/// Override this method to implement the Zenject binding installation logic.
		/// </summary>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		protected async virtual Task Install()
		{
			// Implementation intentionally left blank.
		}
	}
}