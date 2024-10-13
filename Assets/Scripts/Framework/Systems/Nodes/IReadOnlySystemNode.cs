using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IReadOnlySystemNode<TSystem>
	{
		bool IsDetached { get; }

		sbyte Priority { get; }

		byte ExpectedThread { get; }

		TSystem System { get; }

		Type SystemType { get; }

		#region In

		IReadOnlySystemNode<TSystem> SequentialPrevious { get; }

		IEnumerable<IReadOnlySystemNode<TSystem>> ParallelPrevious { get; }
		
		#endregion

		#region Out

		IReadOnlySystemNode<TSystem> SequentialNext { get; }

		IEnumerable<IReadOnlySystemNode<TSystem>> ParallelNext { get; }

		#endregion
	}
}