using System;

namespace HereticalSolutions
{
	public static class DelegateExtensions
	{
		public static object[] CastInvocationListToObjects(this Delegate[] invocationList)
		{
			object[] result = new object[invocationList.Length];

			for (int i = 0; i < invocationList.Length; i++)
			{
				result[i] = (object)invocationList[i];
			}

			return result;
		}

		public static Action[] CastInvocationListToActions(this Delegate[] invocationList)
		{
			Action[] result = new Action[invocationList.Length];

			for (int i = 0; i < invocationList.Length; i++)
			{
				result[i] = (Action)invocationList[i];
			}

			return result;
		}

		public static Action<T>[] CastInvocationListToGenericActions<T>(this Delegate[] invocationList)
		{
			Action<T>[] result = new Action<T>[invocationList.Length];

			for (int i = 0; i < invocationList.Length; i++)
			{
				result[i] = (Action<T>)invocationList[i];
			}

			return result;
		}
	}
}