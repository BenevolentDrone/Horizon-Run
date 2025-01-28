using System;
using System.Collections.Generic;

namespace HereticalSolutions.Allocations
{
	public class GUIDAllocationController
		: IIDAllocationController<Guid>
	{
		public static readonly Guid INVALID_VALUE = default(Guid);

		private readonly HashSet<Guid> allocatedIDs;

		public GUIDAllocationController(
			HashSet<Guid> allocatedIDs)
		{
			this.allocatedIDs = allocatedIDs;
		}

		#region IIDAllocationController

		public bool ValidateID(
			Guid id)
		{
			return id != INVALID_VALUE;
		}

		public bool IsAllocated(
			Guid id)
		{
			return allocatedIDs.Contains(id);
		}

		public bool AllocateID(
			out Guid id)
		{
			do
			{
				id = Guid.NewGuid();
			}
			while (IsAllocated(id));

			allocatedIDs.Add(id);

			return true;
		}

		public bool FreeID(
			Guid id)
		{
			if (!IsAllocated(id))
			{
				return false;
			}

			allocatedIDs.Remove(id);

			return true;
		}

		#endregion

		public static Guid AllocateGUIDStatic()
		{
			return Guid.NewGuid();
		}
	}
}