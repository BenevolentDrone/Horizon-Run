using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Systems;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class DefaultECSSystemBuilder<TSystem, TProcedure>
		: ASystemBuilder<TSystem, TProcedure>
	{
		public DefaultECSSystemBuilder(
			HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			IRepository<string, IStageNode<TProcedure>> stages,
			IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

			IStageNode<TProcedure> startNode,
			IStageNode<TProcedure> finishNode,

			byte freeThreadIndex,

			ILogger logger)
			: base(
				allProcedureNodes,
				stages,
				procedures,

				startNode,
				finishNode,

				freeThreadIndex,

				logger)
		{
		}

		#region ISystemBuilder

		public override bool BuildSystem(
			out TSystem system)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}