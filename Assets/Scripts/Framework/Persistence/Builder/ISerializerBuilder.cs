namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilder
	{
		ISerializerBuilder NewSerializer();

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

		ISerializerBuilder WithPath();

		ISerializerBuilder WithAppend();

		ISerializerBuilder WithReadWrite();

		ISerializerBuilder WithBlockSerialization();

		ISerializerBuilder With<TArgument>();

		#endregion

		ISerializer Build();
	}
}