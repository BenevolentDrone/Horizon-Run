using System;
using System.Collections.Generic;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Logging
{
	public class LoggerWrapperWithFileDump
		: ILogger,
		  ILoggerWrapper,
		  IDumpable
	{
		private readonly ILogger innerLogger;

		private readonly ISerializationArgument serializationArgument;

		private readonly ISerializer serializer;

		private readonly List<string> fullLog;

		public LoggerWrapperWithFileDump(
			ILogger innerLogger,
			ISerializationArgument serializationArgument,
			ISerializer serializer,
			List<string> fullLog)
		{
			this.innerLogger = innerLogger;

			this.serializationArgument = serializationArgument;

			this.serializer = serializer;

			this.fullLog = fullLog;
		}

		#region ILoggerWrapper

		public ILogger InnerLogger { get => innerLogger; }

		#endregion

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
                
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log(value);
		}

		public void Log<TSource>(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log(
				logSource,
				value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log(
				value,
				arguments);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log<TSource>(
				value,
				arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.Log(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning(
				logSource,
				value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning(
				value,
				arguments);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning<TSource>(
				value,
				arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogWarning(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError(
				logSource,
				value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError(
				value,
				arguments);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError<TSource>(
				value,
				arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			innerLogger.LogError(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			var result = innerLogger.FormatException(value);

			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			return result;	
		}

		public string FormatException<TSource>(
			string value)
		{
			var result = innerLogger.FormatException<TSource>(value);

			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			return result;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			var result = innerLogger.FormatException(				
				logSource,
				value);

			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = true;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"{value}\n");
			}
			else
			{
				fullLog.Add(value);
			}

			return result;
		}

		#endregion

		#endregion

		#region Dumpable

		public void Dump()
		{
			Log(
				GetType(),
				$"DUMPING LOGS TO FILE");

			if (serializationArgument is StreamArgument streamArgument)
			{
				streamArgument.KeepOpen = false;
				
				serializer.Serialize<string>(
					serializationArgument,
					$"\n");
			}
			else
			{
				serializer.Serialize<string[]>(
					serializationArgument,
					fullLog.ToArray());
			}
		}

		#endregion
	}
}