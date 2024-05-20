using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace HereticalSolutions.Persistence.Serializers
{
	//Courtesy of https://stackoverflow.com/questions/39383098/ignore-missing-types-during-deserialization-of-list
	public class JsonSerializationBinderException : JsonSerializationException
	{
		public JsonSerializationBinderException() { }

		public JsonSerializationBinderException(string message) : base(message) { }

		public JsonSerializationBinderException(string message, Exception innerException) : base(message, innerException) { }

		public JsonSerializationBinderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}