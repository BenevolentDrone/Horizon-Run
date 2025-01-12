using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public interface IBPlusTree<T>
	{
		void Insert(
			T key);
		
		bool Search(
			T key);

		bool Remove(
			T key);

		int Count { get; }

		IEnumerable<T> All { get; }

		//Suggested by ChatGPT. ¯\_(ツ)_/¯
		void InOrderTraversal(
			Action<T> action);

		void Clear();
	}
}