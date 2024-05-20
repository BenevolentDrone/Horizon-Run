using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Time
{
	public class RuntimeTimerMetadata
		: IContainsRuntimeTimer,
		  ICleanUppable,
		  IDisposable
	{
		public RuntimeTimerMetadata()
		{
			RuntimeTimer = null;
		}

		#region IContainsRuntimeTimer

		public IRuntimeTimer RuntimeTimer { get; set; }

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (RuntimeTimer is ICleanUppable)
				(RuntimeTimer as ICleanUppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (RuntimeTimer is IDisposable)
				(RuntimeTimer as IDisposable).Dispose();
		}

		#endregion
	}
}