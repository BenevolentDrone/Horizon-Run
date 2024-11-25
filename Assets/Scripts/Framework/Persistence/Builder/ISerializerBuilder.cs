using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence
{
	public interface ISerializerBuilder
	{
		ISerializerBuilder NewSerializer(
			bool append = false);

		#region Settings

		ISerializerBuilder FromAbsolutePath(
			FilAtAbsolutePath filePath);

		ISerializerBuilder FromRelativePath(
			FileAtRelativePath filePath);

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

		#endregion

		#region Serialization strategies

		ISerializerBuilder AsString();

		ISerializerBuilder AsTextFile();

		ISerializerBuilder AsBinaryFile();

		ISerializerBuilder AsTextStream();

		ISerializerBuilder AsFileStream();

		#endregion

		ISerializer Build();
	}
}