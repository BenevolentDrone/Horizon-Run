using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;


namespace HereticalSolutions.Modules.Core_DefaultECS.Factories
{
	public static class DefaultECSSystemFactory
	{
		public static DefaultECSSystemBuilder<TSystem, TProcedure> BuildDefaultECSSystemBuilder<TSystem, TProcedure>(
			ILoggerResolver loggerResolver)
		{
			SystemFactory.PrepareSystemBuilderDependencies<TSystem, TProcedure>(
				out HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
				out IRepository<string, IStageNode<TProcedure>> stages,
				out IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

				out IStageNode<TProcedure> startNode,
				out IStageNode<TProcedure> finishNode,

				loggerResolver);

			return new DefaultECSSystemBuilder<TSystem, TProcedure>(
				allProcedureNodes,
				stages,
				procedures,

				startNode,
				finishNode,

				logger: loggerResolver?.GetLogger<DefaultECSSystemBuilder<TSystem, TProcedure>>());
		}
	}
}