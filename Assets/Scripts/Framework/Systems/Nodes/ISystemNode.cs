using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface ISystemNode<TSystem>
		: IReadOnlySystemNode<TSystem>
	{
		//TODO: add the 'tree' the node's attached to so that we don't try to detach a node in a wrong tree

		new bool IsDetached { get; set; }

		new byte ExpectedThread { get; set; }

		#region In

		new IReadOnlySystemNode<TSystem> SequentialPrevious { get; set; }

		new IEnumerable<IReadOnlySystemNode<TSystem>> ParallelPrevious { get; set; }

		#endregion

		#region Out

		new IReadOnlySystemNode<TSystem> SequentialNext { get; set; }

		new IEnumerable<IReadOnlySystemNode<TSystem>> ParallelNext { get; set; }

		#endregion
	}
}