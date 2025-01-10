using System;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging.Factories;

namespace HereticalSolutions.Logging
{
	public class LoggerBuilder
		: ILoggerBuilder
	{
		private IRepository<Type, bool> explicitLogSourceRules;

		private ILogger rootLogger;

		private ILogger currentLogger;

		private bool allowedByDefault;

		public LoggerBuilder()
		{
			explicitLogSourceRules = null;

			rootLogger = null;

			currentLogger = null;

			allowedByDefault = true;
		}

		#region ILoggerBuilder

		public ILogger RootLogger { get => rootLogger; }

		public ILogger CurrentLogger
		{
			get => currentLogger;
			set
			{
				currentLogger = value;

				if (rootLogger == null)
				{
					rootLogger = currentLogger;
				}
			}
		}

		public bool CurrentAllowedByDefault { get => allowedByDefault; }


		public ILoggerBuilder NewLogger()
		{
			explicitLogSourceRules = RepositoriesFactory.BuildDictionaryRepository<Type, bool>();

			rootLogger = null; //LoggersFactory.BuildProxyWrapper();

			currentLogger = rootLogger;

			allowedByDefault = true;

			return this;
		}


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


		public ILoggerBuilder AddSink(
			ILoggerSink loggerSink)
		{
			if (currentLogger is ICompositeLoggerWrapper compositeLogger)
			{
				compositeLogger.InnerLoggers.Add(loggerSink);
			}
			else if (currentLogger is ILoggerWrapper loggerWrapper)
			{
				loggerWrapper.InnerLogger = loggerSink;
			}

			CurrentLogger = loggerSink;

			return this;
		}

		public ILoggerBuilder AddWrapperBelow(
			ILoggerWrapper loggerWrapper)
		{
			if (currentLogger is ICompositeLoggerWrapper compositeLogger)
			{
				compositeLogger.InnerLoggers.Add(loggerWrapper);
			}
			else if (currentLogger is ILoggerWrapper currentLoggerWrapper)
			{
				currentLoggerWrapper.InnerLogger = loggerWrapper;
			}

			CurrentLogger = loggerWrapper;

			return this;
		}

		public ILoggerBuilder Branch()
		{
			var compositeLogger = LoggerFactory.BuildCompositeLoggerWrapper();

			if (currentLogger is ICompositeLoggerWrapper currentCompositeLogger)
			{
				currentCompositeLogger.InnerLoggers.Add(compositeLogger);
			}
			else if (currentLogger is ILoggerWrapper loggerWrapper)
			{
				loggerWrapper.InnerLogger = compositeLogger;
			}

			CurrentLogger = compositeLogger;

			return this;
		}


		public ILoggerResolver Build()
		{
			return LoggerFactory.BuildSharedLoggerResolver(
				rootLogger,
				explicitLogSourceRules,
				allowedByDefault);
		}

		#endregion
	}
}