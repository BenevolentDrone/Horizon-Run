using System;

using Newtonsoft.Json.Serialization;

namespace HereticalSolutions.Persistence.Serializers
{
	//Courtesy of https://stackoverflow.com/questions/39383098/ignore-missing-types-during-deserialization-of-list
	public class JsonSerializationBinder : ISerializationBinder
	{
		readonly ISerializationBinder binder;

		public JsonSerializationBinder(ISerializationBinder binder)
		{
			if (binder == null)
				throw new ArgumentNullException();
			this.binder = binder;
		}

		public Type BindToType(string assemblyName, string typeName)
		{
			try
			{
				return binder.BindToType(assemblyName, typeName);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationBinderException(ex.Message, ex);
			}
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			binder.BindToName(serializedType, out assemblyName, out typeName);
		}
	}
}