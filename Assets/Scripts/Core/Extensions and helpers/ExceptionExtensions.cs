using System;
using System.Collections.Generic;

namespace HereticalSolutions
{
	public static class ExceptionExtensions
	{
		//Courtesy of https://stackoverflow.com/questions/39383098/ignore-missing-types-during-deserialization-of-list
		public static IEnumerable<Exception> InnerExceptionsAndSelf(this Exception ex)
		{
			while (ex != null)
			{
				yield return ex;
				
				ex = ex.InnerException;
			}
		}
	}
}