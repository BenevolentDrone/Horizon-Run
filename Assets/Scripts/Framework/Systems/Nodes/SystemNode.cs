using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public class SystemNode<TSystem>
		: ISystemNode<TSystem>
	{
		public SystemNode(
			TSystem system,
			sbyte priority = 0)
		{
			System = system;

			Priority = priority;
		}

		#region ISystemNode

		public bool IsDetached { get; set; }

		public sbyte Priority { get; private set; }

		public byte ExpectedThread { get; set; }

		public TSystem System { get; private set; }

		public Type SystemType 
		{
			get
			{
				return System != null ? System.GetType() : null;
			}
		}

		#region In

		public IReadOnlySystemNode<TSystem> SequentialPrevious { get; set; }

		public IEnumerable<IReadOnlySystemNode<TSystem>> ParallelPrevious { get; set; }

		#endregion

		#region Out

		public IReadOnlySystemNode<TSystem> SequentialNext { get; set; }

		public IEnumerable<IReadOnlySystemNode<TSystem>> ParallelNext { get; set; }

		#endregion

		#endregion
	}
}