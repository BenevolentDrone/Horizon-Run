using HereticalSolutions.Metadata.Factories;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class SerializerBuilder
		: ISerializerBuilder
	{
		private readonly ILogger logger;

		private ISerializerContext serializerContext;

		public SerializerBuilder()
		{
			serializerContext = null;
		}

		#region ISerializerBuilder

		ISerializerBuilder NewSerializer()
		{
			serializerContext = new SerializerContext();

			return this;
		}

		#region Settings

		ISerializerBuilder FromAbsolutePath(
			FileAtAbsolutePathSettings filePathSettings);

		ISerializerBuilder FromRelativePath(
			FileAtRelativePathSettings filePathSettings);

		ISerializerBuilder FromTempPath(
			FileAtTempPathSettings filePathSettings);

		ISerializerBuilder From<TPathSettings>(
			TPathSettings pathSettings);

		#endregion

		#region Format serializer

		ISerializerBuilder ToObject();

		ISerializerBuilder ToPlainText();

		ISerializerBuilder ToJSON();

		ISerializerBuilder ToCSV(
			bool includeHeader = true);

		ISerializerBuilder ToYAML();

		ISerializerBuilder ToProtobuf();

		ISerializerBuilder ToXML();

		ISerializerBuilder ToBinary();

		ISerializerBuilder To<TSerializationFormat>();

		#endregion

		#region Serialization strategies

		ISerializerBuilder AsString();

		ISerializerBuilder AsTextFile();

		ISerializerBuilder AsBinaryFile();

		ISerializerBuilder AsTextStream();

		ISerializerBuilder AsFileStream();

		ISerializerBuilder AsMemoryStream();

		ISerializerBuilder AsIsolatedStorageFileStream();

		ISerializerBuilder As<TMedia>();

		#endregion

		#region Arguments

		ISerializerBuilder WithPath()
		{
			if (serializerContext == null)
			{

			}

			EnsureArgumentsExist();

			serializerContext.Arguments.Add(new PathArgument());

			return this;
		}

		ISerializerBuilder WithAppend();

		ISerializerBuilder WithReadWrite();

		ISerializerBuilder WithBlockSerialization();

		ISerializerBuilder With<TArgument>();

		#endregion

		ISerializer Build();

		#endregion

		private void EnsureArgumentsExist()
		{
			if (serializerContext.Arguments == null)
				serializerContext.Arguments = MetadataFactory.BuildStronglyTypedMetadata();
		}
	}
}