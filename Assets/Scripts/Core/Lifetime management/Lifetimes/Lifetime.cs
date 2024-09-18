using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement
{
	public class Lifetime
		: ILifetimeable,
		  ISetUppable,
		  IInitializable,
		  ICleanuppable,
		  ITearDownable,
		  IDisposable
	{
		private readonly EInitializationFlags initializationFlags;
		
		private readonly ILogger logger;

		
		private Action setUp;

		private Func<object[], bool> initialize;
		
		private Action cleanup;
		
		private Action tearDown;
		
		public Lifetime(
			EInitializationFlags initializationFlags = 
				EInitializationFlags.NO_ARGS_ALLOWED,
			
			Action setUp = null,
			Func<object[], bool> initialize = null,
			Action cleanup = null,
			Action tearDown = null,
			
			ILogger logger = null)
		{
			this.logger = logger;
			
			this.initializationFlags = initializationFlags;
			
			this.setUp = setUp;
			
			this.initialize = initialize;
			
			this.cleanup = cleanup;
			
			this.tearDown = tearDown;
		}

		#region ILifetimeable

		public bool IsSetUp { get; private set; } = false;

		public bool IsInitialized { get; private set; } = false;

		public Action<ILifetimeable> OnSetUp { get; set; }
		
		public Action<ILifetimeable, object[]> OnInitialized { get; set; }
		
		public EInitializationFlags InitializationFlags
		{
			get => initializationFlags;
		}

		public Action<ILifetimeable> OnCleanedUp { get; set; }

		public Action<ILifetimeable> OnTornDown { get; set; }

		#endregion

		#region ISetUppable

		public void SetUp()
		{
			if (IsSetUp)
			{
				return;
			}

			setUp?.Invoke();

			IsSetUp = true;
			
			logger?.Log<Lifetime>(
				"SET UP");
			
			OnSetUp?.Invoke(this);
		}

		#endregion

		#region IInitializable

		public bool Initialize(
			object[] args = null)
		{
			if ((args == null || args.Length == 0)
			    && !HasFlags(
				    initializationFlags,
				    EInitializationFlags.NO_ARGS_ALLOWED))
			{
				logger?.LogError<Lifetime>(
					"ATTEMPT TO INITIALIZE WITH EMPTY ARGS AND FLAG \"NO_ARGS_ALLOWED\" NOT SET");

				return false;
			}
            
			if (IsInitialized)
			{
				return true;
			}

			if (!IsSetUp)
			{
				SetUp();
			}

			if (initialize != null
			    && !initialize.Invoke(args))
			{
				return false;
			}

			IsInitialized = true;
			
			logger?.Log<Lifetime>(
				"INITIALIZED");

			OnInitialized?.Invoke(this, args);

			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (!IsInitialized)
				return;

			cleanup?.Invoke();

			IsInitialized = false;
			
			logger?.Log<Lifetime>(
				"CLEANED UP");

			OnCleanedUp?.Invoke(this);
		}

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			if (IsInitialized)
				Cleanup();
            
			tearDown?.Invoke();
			
			IsSetUp = false;
			
			logger?.Log<Lifetime>(
				"TORN DOWN");

			OnTornDown?.Invoke(this);

			
			OnSetUp = null;
            
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
		
		private static bool HasFlags(
			EInitializationFlags currentFlags,
			EInitializationFlags flagsToCheckAgainst)
		{
			return (currentFlags & flagsToCheckAgainst) != 0;
		}
	}
}