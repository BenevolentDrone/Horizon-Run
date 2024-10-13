using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public class StageNode<TSystem>
		: ISystemNode<TSystem>,
		  IStageNode<TSystem>
	{
		public StageNode(
			string stage,
			TSystem system,
			sbyte priority = 0)
		{
			Stage = stage;

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
	
		#region IStageNode

		public string Stage { get; private set; }

		#endregion
	}
}