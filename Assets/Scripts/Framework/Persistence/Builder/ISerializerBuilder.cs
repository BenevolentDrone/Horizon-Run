namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilder
	{
		ISerializerBuilder NewSerializer(
			bool append = false);

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

		ISerializerBuilder AsIsolatedStorageFileStream();

		ISerializerBuilder As<TMedia>();

		#endregion

		ISerializer Build();
	}
}