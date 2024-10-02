using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStreamStrategy
		: IPlainTextSerializationStrategy
	{
		private readonly ILogger logger;

		public SerializePlainTextIntoStreamStrategy(
			ILogger logger = null)
		{
			this.logger = logger;
		}
		
		public bool StartSerialization(ISerializationArgument argument)
		{
			StreamArgument streamArgument = (StreamArgument)argument;
			
			if (streamArgument.Writer != null)
				return true;
            
			FilePathSettings filePathSettings = streamArgument.Settings;
            
			if (!StreamIO.OpenWriteStream(
				filePathSettings,
				out StreamWriter streamWriter,
				logger))
				return false;
			
			streamArgument.Writer = streamWriter;

			return true;
		}

		public bool Serialize(
			ISerializationArgument argument,
			string text)
		{
			if (!StartSerialization(argument))
				return false;
			
			StreamArgument streamArgument = (StreamArgument)argument;
			
			if (streamArgument.Writer == null)
			{
				return false;
			}

			streamArgument.Writer.Write(text);

			if (streamArgument.KeepOpen)
				return true;
			
			if (!FinishSerialization(argument))
				return false;

			return true;
		}
		
		public bool FinishSerialization(ISerializationArgument argument)
		{
			StreamArgument streamArgument = (StreamArgument)argument;

			if (streamArgument.Writer == null)
				return true;
			
			StreamIO.CloseStream(streamArgument.Writer);
            
			streamArgument.Writer = null;

			return true;
		}

		public bool Deserialize(
			ISerializationArgument argument,
			out string text)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			text = string.Empty;

			if (!StreamIO.OpenReadStream(
				filePathSettings,
				out StreamReader streamReader,
				logger))
				return false;

			text = streamReader.ReadToEnd();

			StreamIO.CloseStream(streamReader);

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			StreamIO.Erase(filePathSettings);
		}
	}
}