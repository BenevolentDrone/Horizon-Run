using System;
using System.Collections.Generic;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Logging
{
	public class FileSink
		: ILoggerSink,
		  IDumpable
	{
		private readonly ISerializationArgument serializationArgument;

		private readonly ISerializer serializer;

		private readonly List<string> fullLog;

		public FileSink(
			ISerializationArgument serializationArgument,
			ISerializer serializer,
			List<string> fullLog)
		{
			this.serializationArgument = serializationArgument;

			this.serializer = serializer;

			this.fullLog = fullLog;
		}

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
		}

		#endregion

		#region Exception

		public string FormatException(
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

			return value;	
		}

		public string FormatException<TSource>(
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

			return value;
		}

		public string FormatException(
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

			return value;
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