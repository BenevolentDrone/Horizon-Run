using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public class ProcedureNode<TProcedure>
		: IProcedureNode<TProcedure>
	{
		public ProcedureNode(
			TProcedure procedure,
			sbyte priority = 0)
		{
			Procedure = procedure;

			Priority = priority;

			IsDetached = true;

			ExpectedThread = 0;
		}

		#region IProcedureNode

		public bool IsDetached { get; set; }

		public sbyte Priority { get; private set; }

		public byte ExpectedThread { get; set; }

		public TProcedure Procedure { get; set; }

		public Type ProcedureType 
		{
			get
			{
				return Procedure != null ? Procedure.GetType() : null;
			}
		}

		#region In

		public IReadOnlyProcedureNode<TProcedure> SequentialPrevious { get; set; }

		public IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelPrevious { get; set; }

		#endregion

		#region Out

		public IReadOnlyProcedureNode<TProcedure> SequentialNext { get; set; }

		public IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelNext { get; set; }

		#endregion

		#endregion
	}
}