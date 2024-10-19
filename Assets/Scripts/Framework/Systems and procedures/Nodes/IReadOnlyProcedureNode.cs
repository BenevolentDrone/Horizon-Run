using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IReadOnlyProcedureNode<TProcedure>
	{
		bool IsDetached { get; }

		sbyte Priority { get; }

		byte ExpectedThread { get; }

		TProcedure Procedure { get; }

		Type ProcedureType { get; }

		#region In

		IReadOnlyProcedureNode<TProcedure> SequentialPrevious { get; }

		IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelPrevious { get; }
		
		#endregion

		#region Out

		IReadOnlyProcedureNode<TProcedure> SequentialNext { get; }

		IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelNext { get; }

		#endregion
	}
}