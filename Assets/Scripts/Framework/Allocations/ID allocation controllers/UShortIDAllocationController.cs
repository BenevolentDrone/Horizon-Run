using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Allocations
{
	public class UShortIDAllocationController
		: IIDAllocationController<ushort>
	{
		public const ushort INVALID_VALUE = 0;

		private readonly Queue<ushort> freeIDs;

		private readonly ILogger logger;

		private ushort lastAllocatedID;

		public UShortIDAllocationController(
			Queue<ushort> freeIDs,
			ILogger logger)
		{
			this.freeIDs = freeIDs;

			this.logger = logger;

			lastAllocatedID = INVALID_VALUE;
		}

		#region IIDAllocationController

		public bool ValidateID(
			ushort id)
		{
			return id != INVALID_VALUE;
		}

		public bool IsAllocated(
			ushort id)
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
			out ushort id)
		{
			if (freeIDs.Count > 0)
			{
				id = freeIDs.Dequeue();

				return true;
			}

			if (lastAllocatedID == ushort.MaxValue)
			{
				logger.LogError(
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
			ushort id)
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