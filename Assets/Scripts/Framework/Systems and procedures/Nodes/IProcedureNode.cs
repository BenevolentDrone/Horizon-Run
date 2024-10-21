using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IProcedureNode<TProcedure>
		: IReadOnlyProcedureNode<TProcedure>
	{
		new bool IsDetached { get; set; }

		new byte ExpectedThread { get; set; }

		new TProcedure Procedure { get; set; }

		#region In

		new IReadOnlyProcedureNode<TProcedure> SequentialPrevious { get; set; }

		new IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelPrevious { get; set; }

		#endregion

		#region Out

		new IReadOnlyProcedureNode<TProcedure> SequentialNext { get; set; }

		new IEnumerable<IReadOnlyProcedureNode<TProcedure>> ParallelNext { get; set; }

		#endregion
	}
}