using System;
using System.Collections.Generic;

using HereticalSolutions.Logging.Factories;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging
{
	public class LoggerBuilder
		: ILoggerBuilder,
		  ILoggerResolver
	{
		private readonly IRepository<Type, bool> explicitLogSourceRules;

		private ILogger currentLogger;

		private bool allowedByDefault;

		public LoggerBuilder(
			IRepository<Type, bool> explicitLogSourceRules)
		{
			this.explicitLogSourceRules = explicitLogSourceRules;

			currentLogger = null;

			allowedByDefault = true;
		}

		#region ILoggerBuilder

		public ILogger CurrentLogger { get => currentLogger; }

		public bool CurrentAllowedByDefault { get => allowedByDefault; }

		public ILoggerBuilder ToggleAllowedByDefault(
			bool allowed)
		{
			allowedByDefault = allowed;

			return this;
		}

		public ILoggerBuilder ToggleLogSource<TLogSource>(
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				typeof(TLogSource),
				allowed);

			return this;
		}

		public ILoggerBuilder ToggleLogSource(
			Type logSourceType,
			bool allowed)
		{
			explicitLogSourceRules.AddOrUpdate(
				logSourceType,
				allowed);

			return this;
		}

		public ILoggerBuilder AddSink(ILoggerSink loggerSink)
		{
			if (currentLogger != null)
				throw new Exception("CURRENT LOGGER IS NOT EMPTY");

			currentLogger = loggerSink;

			return this;
		}

		public ILoggerBuilder Wrap(ILoggerWrapper loggerWrapper)
		{
			if (currentLogger == null)
				throw new Exception("NO LOGGER TO WRAP");

			currentLogger = loggerWrapper;

			return this;
		}

		public ILoggerBuilder Branch(IEnumerable<ILogger> siblingLoggers)
		{
			if (currentLogger == null)
				throw new Exception("NO LOGGER TO BRANCH FROM");

			IEnumerable<ILogger> innerLoggers = null;

			if (siblingLoggers is List<ILogger> siblingsList)
			{
				siblingsList.Add(currentLogger);

				innerLoggers = siblingLoggers;
			}
			else
			{
				var siblings = new List<ILogger>(siblingLoggers);

				siblings.Add(currentLogger);

				innerLoggers = siblings.ToArray();
			}

			if (innerLoggers == null)
				throw new Exception("INVALID INNER LOGGERS COLLECTION");

			currentLogger = LoggersFactory.BuildCompositeLoggerWrapper(innerLoggers);

			return this;
		}

		#endregion

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
				return currentLogger;
			}

			return null;
		}

		#endregion
	}
}