/*
using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadWriteResourceStorageHandle<TResource>
		: AReadOnlyResourceStorageHandle<TResource>,
		  IResourceStorageHandle
	{
		public AReadWriteResourceStorageHandle(
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
			: base(
				runtimeResourceManager,
				logger)
		{
		}

		#region IResourceStorageHandle

		public bool SetRawResource(object rawResource)
		{
			if (!allocated)
			{
				return false;
			}

			this.resource = (TResource)rawResource;

			return true;
		}

		public bool SetResource<TValue>(TValue resource)
		{
			if (!allocated)
			{
				return false;
			}

			switch (resource)
			{
				case TResource targetTypeResource:

					this.resource = targetTypeResource;

					break;

				default:

					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"CANNOT SET RESOURCE OF TYPE {nameof(TValue)} TO RESOURCE OF TYPE {nameof(TResource)}"));
			}

			return true;
		}

		#endregion
	}
}
*/