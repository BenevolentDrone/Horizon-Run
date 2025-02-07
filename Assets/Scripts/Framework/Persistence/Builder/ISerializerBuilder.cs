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
			TPathSettings pathSettings)
			where TPathSettings : IPathSettings;

		#endregion

		#region Format serializer

		ISerializerBuilder ToObject();

		ISerializerBuilder ToBinary();

		ISerializerBuilder To<TFormatSerializer>(
			object[] arguments)
			where TFormatSerializer : IFormatSerializer;

#endregion

		#region Serialization strategies

		ISerializerBuilder AsString();

		ISerializerBuilder AsTextFile();

		ISerializerBuilder AsBinaryFile();

		ISerializerBuilder AsTextStream(
			bool flushAutomatically = true);

		ISerializerBuilder AsFileStream(
			bool flushAutomatically = true);

		ISerializerBuilder AsMemoryStream(
			byte[] buffer = null,
			int index = -1,
			int count = -1);

		ISerializerBuilder AsIsolatedStorageFileStream(
			bool flushAutomatically = true);

		ISerializerBuilder As<TSerializationStrategy>(
			object[] arguments)
			where TSerializationStrategy : ISerializationStrategy;

		#endregion

		#region Arguments

		ISerializerBuilder WithPath(
			string path);

		ISerializerBuilder WithAppend();

		ISerializerBuilder WithReadWriteAccess();

		ISerializerBuilder WithBlockSerialization(
			int blockSize = 1024);

		ISerializerBuilder With<TInterface, TArgument>(
			object[] arguments)
			where TArgument : TInterface;

		#endregion

		#region Visitor

		ISerializerBuilder WithVisitor(
			IVisitor visitor);

		#endregion

		ISerializer Build();
	}
}