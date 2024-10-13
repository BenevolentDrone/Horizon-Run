using System;

namespace HereticalSolutions
{
	public static class DelegateExtensions
	{
		public static object[] CastInvokationListToObjects(this Delegate[] invokationList)
		{
			object[] result = new object[invokationList.Length];

			for (int i = 0; i < invokationList.Length; i++)
			{
				result[i] = (object)invokationList[i];
			}

			return result;
		}

		public static Action[] CastInvokationListToActions(this Delegate[] invokationList)
		{
			Action[] result = new Action[invokationList.Length];

			for (int i = 0; i < invokationList.Length; i++)
			{
				result[i] = (Action)invokationList[i];
			}

			return result;
		}

		public static Action<T>[] CastInvokationListToGenericActions<T>(this Delegate[] invokationList)
		{
			Action<T>[] result = new Action<T>[invokationList.Length];

			for (int i = 0; i < invokationList.Length; i++)
			{
				result[i] = (Action<T>)invokationList[i];
			}

			return result;
		}
	}
}