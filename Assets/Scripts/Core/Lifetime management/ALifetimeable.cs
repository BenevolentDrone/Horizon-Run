using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement
{
	public abstract class ALifetimeable
		: ILifetimeable,
		  ISetUppable,
		  //IInitializable,
		  ICleanUppable,
		  ITearDownable,
		  IDisposable
	{
		protected readonly ILogger logger;

		public ALifetimeable(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		#region ILifetimeable

		public bool IsSetUp { get; protected set; } = false;

		public bool IsInitialized { get; protected set; } = false;

		public Action OnInitialized { get; set; }

		public Action OnCleanedUp { get; set; }

		public Action OnTornDown { get; set; }

		#endregion

		#region ISetUppable

		public virtual void SetUp()
		{
			if (IsSetUp)
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"ALREADY SET UP"));

			//Set up
			SetUpInternal();

			IsSetUp = true;
		}

		#endregion

		/*
		#region IInitializable

		public virtual void Initialize(object[] args = null)
		{
			ValidateInitialization();

			//Initialization
			InitializeInternal(args);

			FinalizeInitialization();
		}

		#endregion
		*/

		#region ICleanUppable

		public virtual void Cleanup()
		{
			if (!IsInitialized)
				return;

			//Clean up
			CleanupInternal();

			IsInitialized = false;

			OnCleanedUp?.Invoke();
		}

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			IsSetUp = false;

			Cleanup();

			//Tear down
			TearDownInternal();


			OnTornDown?.Invoke();

			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			TearDown();
		}

		#endregion

		protected void ValidateInitialization()
		{
			if (!IsSetUp)
			{
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"LIFETIMEABLE SHOULD BE SET UP BEFORE BEING INITIALIZED"));
			}

			if (IsInitialized)
			{
				throw new Exception(
					logger.TryFormat(
						GetType(),
						$"INITIALIZING LIFETIMEABLE THAT IS ALREADY INITIALIZED"));
			}
		}

		protected void FinalizeInitialization()
		{
			IsInitialized = true;

			OnInitialized?.Invoke();
		}

		protected virtual void SetUpInternal()
		{

		}

		protected virtual void CleanupInternal()
		{

		}

		protected virtual void TearDownInternal()
		{

		}
	}
}