using System;

namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilder
	{
		ISerializerBuilder NewSerializer();

		ISerializerBuilder RecycleSerializer(
			ISerializer serializer);

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

		ISerializerBuilder ErasePath();

		#endregion

		#region Data converters

		ISerializerBuilder WithByteArrayFallback();

		ISerializerBuilder WithDataConverter<TDataConverter>(
			object[] arguments)
			where TDataConverter : IDataConverter;

		ISerializerBuilder EraseDataConverters();

		#endregion

		#region Format serializer

		ISerializerBuilder ToObject();

		ISerializerBuilder ToBinary();

		ISerializerBuilder To<TFormatSerializer>(
			object[] arguments)
			where TFormatSerializer : IFormatSerializer;

		ISerializerBuilder EraseFormatSerializer();

		#endregion

		#region Serialization strategies

		ISerializerBuilder AsString(
			Func<string> valueGetter,
			Action<string> valueSetter);

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

		ISerializerBuilder EraseStrategy();

		#endregion

		#region Arguments

		ISerializerBuilder WithPath(
			string path);

		ISerializerBuilder WithAppend();

		ISerializerBuilder WithReadWriteAccess();

		ISerializerBuilder WithBlockSerialization();

		ISerializerBuilder With<TInterface, TArgument>(
			object[] arguments)
			where TArgument : TInterface;

		ISerializerBuilder Without<TInterface>();

		ISerializerBuilder EraseArguments();

		#endregion

		#region Visitor

		ISerializerBuilder WithVisitor(
			IVisitor visitor);

		ISerializerBuilder EraseVisitor();

		#endregion

		#region Build

		Serializer BuildSerializer();

		ConcurrentSerializer BuildConcurrentSerializer();

		#endregion

		void Refurbish();
	}
}