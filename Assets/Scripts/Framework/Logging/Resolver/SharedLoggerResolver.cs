using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging
{
	public class SharedLoggerResolver
		: ILoggerResolver
	{
		private readonly ILogger rootLogger;

		private readonly IRepository<Type, bool> explicitLogSourceRules;

		private readonly bool allowedByDefault;

		public SharedLoggerResolver(
			ILogger rootLogger,
			IRepository<Type, bool> explicitLogSourceRules,
			bool allowedByDefault)
		{
			this.rootLogger = rootLogger;

			this.explicitLogSourceRules = explicitLogSourceRules;

			this.allowedByDefault = allowedByDefault;
		}

		#region ILoggerResolver

		public ILogger GetLogger<TLogSource>()
		{
			return GetLogger(
				typeof(TLogSource));
		}

		public ILogger GetLogger(Type logSourceType)
		{
			bool provide = allowedByDefault;

			if (logSourceType.IsGenericType)
			{
				var genericTypeDefinition = logSourceType.GetGenericTypeDefinition();

				if (explicitLogSourceRules.Has(
					genericTypeDefinition))
				{
					provide = explicitLogSourceRules.Get(genericTypeDefinition);
				}
				else if (explicitLogSourceRules.Has(
					logSourceType))
				{
					provide = explicitLogSourceRules.Get(logSourceType);
				}
			}
			else
			{
				if (explicitLogSourceRules.Has(
					logSourceType))
				{
					provide = explicitLogSourceRules.Get(logSourceType);
				}
			}

			if (provide)
			{
				return rootLogger;
			}

			return null;
		}

		#endregion
	}
}