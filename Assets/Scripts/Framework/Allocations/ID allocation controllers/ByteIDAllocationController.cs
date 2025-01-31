using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Allocations
{
	public class ByteIDAllocationController
		: IIDAllocationController<byte>
	{
		public const byte INVALID_VALUE = 0;

		private readonly Queue<byte> freeIDs;

		private readonly ILogger logger;

		private byte lastAllocatedID;

		public ByteIDAllocationController(
			Queue<byte> freeIDs,
			ILogger logger)
		{
			this.freeIDs = freeIDs;

			this.logger = logger;

			lastAllocatedID = INVALID_VALUE;
		}

		#region IIDAllocationController

		public bool ValidateID(
			byte id)
		{
			return id != INVALID_VALUE;
		}

		public bool IsAllocated(
			byte id)
		{
			if (id == INVALID_VALUE)
				return false;

			if (id > lastAllocatedID)
			{
				return false;
			}

			if (freeIDs.Contains(
				id))
			{
				return false;
			}

			return true;
		}

		public bool AllocateID(
			out byte id)
		{
			if (freeIDs.Count > 0)
			{
				id = freeIDs.Dequeue();

				return true;
			}

			if (lastAllocatedID == byte.MaxValue)
			{
				logger?.LogError(
					GetType(),
					"NO FREE HANDLES");

				id = INVALID_VALUE;

				return false;
			}
			
			id = lastAllocatedID++;

			lastAllocatedID = id;

			return true;
		}

		public bool FreeID(
			byte id)
		{
			if (!IsAllocated(id))
			{
				return false;
			}

			freeIDs.Enqueue(id);

			return true;
		}

		#endregion
	}
}